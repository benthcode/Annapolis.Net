using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Annapolis.Entity;
using Annapolis.Abstract.Work;
using Annapolis.Abstract.Repository;
using System.Text.RegularExpressions;
using Annapolis.Shared.Model;
using System.Security.Cryptography;
using Annapolis.Shared.Utility;

namespace Annapolis.Work
{
    public class MemberUserWorker : AnnapolisBaseCrudWork<MemberUser>,  IMemberUserWork
    {
        private static readonly int PASSWORD_SALT_SIZE = 24;
       
        private static readonly string REGEX_PATTERN_USERNAME = @"^([\w\d_]|[\u4e00-\u9fa5]){2,18}$";
        private static readonly string REGEX_PATTERN_EMAIL = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        private static readonly string REGEX_PATTERN_PASSWORD = @"(.){6,}";
      
        private readonly IMemberRoleWork _roleWork;
        private readonly ISettingWork _settingWork;
        private readonly IEmailWork _emailWork;
        private readonly ILoggingWork _loggingWork;


        public MemberUserWorker(IMemberRoleWork roleSerivce,
                                 ISettingWork settingWork,
                                 IEmailWork emailWork,
                                 ILoggingWork loggingWork)
        {
            _roleWork = roleSerivce;
            _settingWork = settingWork;
            _emailWork = emailWork;
            _loggingWork = loggingWork;
        }

        public override IQueryable<MemberUser> All
        {
            get
            {
                return Repository.All.Include(x => x.Role);
            }
        }

        public override OperationStatus HasPermission(EntityPermission permission, MemberUser item = null, Guid? threadId = null)
        {
            if (permission == EntityPermission.Add) return OperationStatus.Granted;
            return base.HasPermission(permission, item, threadId);
        }

        #region User Validation & Authentication

        #region Validation

        private OperationStatus CheckValidation(MemberUser user)
        {
            if (!IsValidUserName(user.UserName)) return OperationStatus.InvalidUserName;
            if (!IsValidEmail(user.RegisterEmail)) return OperationStatus.InvalidEmail;
            if (!IsValidPassword(user.Password)) return OperationStatus.InvalidPassword;
            return CheckDuplication(user.UserName, user.RegisterEmail);
        }

        private bool IsValidUserName(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return false;
            return Regex.IsMatch(userName, REGEX_PATTERN_USERNAME);
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            return Regex.IsMatch(email, REGEX_PATTERN_EMAIL);
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            return Regex.IsMatch(password, REGEX_PATTERN_PASSWORD);
        }

        private OperationStatus CheckDuplication(string userName, string registerEmail)
        {
            var existingUsers = Repository.All.Where(x => x.UserName == userName
                                                 || x.RegisterEmail == registerEmail).ToList();

            if (existingUsers.Count(x => x.UserName.Trim().ToLower() == userName.Trim().ToLower()) != 0)
            {
                return OperationStatus.DuplicatedUserName;
            }

            if (existingUsers.Count(x => x.RegisterEmail.Trim().ToLower() == registerEmail.Trim().ToLower()) != 0)
            {
                return OperationStatus.DuplicatedRegisterEmail;
            }

            return OperationStatus.Success;
        }

        #endregion

        private void SanitizeUser(MemberUser user)
        {
            user.UserName = StringUtility.SafePlainText(user.UserName);
            user.RegisterEmail = StringUtility.SafePlainText(user.RegisterEmail);
            user.ContactEmail = StringUtility.SafePlainText(user.ContactEmail);
            user.Password = StringUtility.SafePlainText(user.Password);
            user.PasswordAnswer = StringUtility.SafePlainText(user.PasswordAnswer);
            user.Avatar = StringUtility.SafePlainText(user.Avatar);
            user.Comment = StringUtility.SafePlainText(user.Comment);
            user.PasswordQuestion = StringUtility.SafePlainText(user.PasswordQuestion);
            user.Signature = StringUtility.GetSafeHtml(user.Signature);
        }

        private string CreateSalt(int size)
        {
          
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider(); 
            byte[] saltBytes = new byte[size];
            rng.GetBytes(saltBytes);
            string salt = Convert.ToBase64String(saltBytes);
            return salt;
        }

        private string GenerateSaltedPasswordHash(string passwordPlainText, string salt)
        {

            byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(passwordPlainText + salt);
            byte[] hashBytes = new SHA256Managed().ComputeHash(passwordAndSaltBytes);
            string passwordHash = Convert.ToBase64String(hashBytes);
            return passwordHash;
        }

        private string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }

        #endregion

        public override MemberUser Create()
        {
            MemberUser user = base.Create();

            user.IsApproved = true;
            user.IsLockedOut = false;
            user.IsExternalAccount = false;
            user.CreateDate = DateTime.UtcNow;
            user.RoleId = _settingWork.GetDefaultSetting().NewMemberStartRoleId;

            return user;
        }

        protected override OperationStatus Add(MemberUser item)
        {
            var status = CheckValidation(item);
            if (status != OperationStatus.Success) return status;
            SanitizeUser(item);

            var defaultSetting = _settingWork.GetDefaultSetting();
            #region Set Values
            item.PasswordSalt = CreateSalt(PASSWORD_SALT_SIZE);
            item.Password = GenerateSaltedPasswordHash(item.Password, item.PasswordSalt);
            item.Key = item.UserName;
            item.Token = GenerateToken();
            item.TokenGeneratedTime = DateTime.UtcNow;

            if (defaultSetting.EmailVerifyOnNewUser)
            {
                item.IsApproved = defaultSetting.AutoApproveNewMember;
                item.LastLoginDate = DateTime.UtcNow;
            }
            else
            {
              
                item.IsApproved = true;
            }
            #endregion

            status = base.Add(item);


            #region Send Email
            if (status == OperationStatus.Success)
            {
                if (defaultSetting.EmailVerifyOnNewUser)
                {
                    _emailWork.SendMail(new Email()
                    {
                        EmailFrom = defaultSetting.AdminEmailAddress,
                        EmailTo = item.RegisterEmail,
                        EmailReceiverName = item.UserName,
                        NotificationReadon = EmailNotificationReason.UserCreateVerification
                    });
                }
            }
            #endregion

            return status;
        }

        public bool ValidateUser(string identifier, string password, out MemberUser user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(identifier) || string.IsNullOrWhiteSpace(password))
                {
                    user = null;
                    return false;
                }
                else
                {
                    user = All.Where(x => x.UserName == identifier || x.RegisterEmail == identifier).SingleOrDefault();
                 
                    {
                        return true;
                    }
                    user = null;
                    return false;
                }
            }
            catch
            {
                user = null;
                return false;
            }

        }

        public bool ValidateToken(string username, string token, out MemberUser user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(token))
                {
                    user = null;
                    return false;
                }
                else
                {
                    user = All.Where(x => x.UserName == username && x.Token == token).SingleOrDefault();
                    if (user != null) { return true; }
                    return false;
                }
            }
            catch
            {
                user = null;
                return false;
            }
        }

        public OperationStatus UpdatePassword(string userName, string oldPassword, string newPassword)
        {
            try
            {
                if (!IsValidUserName(userName)) return OperationStatus.InvalidUserName;
                if (!IsValidPassword(newPassword)) return OperationStatus.InvalidPassword;

                MemberUser user;
                if (ValidateUser(userName, oldPassword, out user))
                {
                    user.PasswordSalt = CreateSalt(PASSWORD_SALT_SIZE);
                    user.Password = GenerateSaltedPasswordHash(newPassword, user.PasswordSalt);
                    if (Save(user) == OperationStatus.Success)
                    {
                        return OperationStatus.PasswordUpdateSuccess;
                    }
                    else
                    {
                        return OperationStatus.PasswordUpdateFail;
                    }
                }
                else
                {
                    return OperationStatus.NoUseOrWrongPassword;
                }

            }
            catch
            {
                return OperationStatus.GenericError;
            }
        }
    }
}
