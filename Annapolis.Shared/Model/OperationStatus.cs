namespace Annapolis.Shared.Model
{
    public enum OperationStatus
    {
        None,
        Success,
        Failure,
        DataFormatError,
        GenericError,

        //Permission
        NoPermission,
        Granted,


        //ContentTopic
        TopicHasLocked,
        TopicHasHidden,

        //ContentComment
        CommentHasLocked,
        CommentHasHidden,

        //ContentVote
        VoteHasExisted,
        VoteCannotForYourSelf,

        //Membership 
        SignInSuccess,
        SignOutSuccess,
        RegisterSuccess,
        NoUseOrWrongPassword,
        NotValidUser,

        Verification,
        DuplicatedUserName,
        DuplicatedRegisterEmail,
        DuplicatedContactEmail,
        InvalidUserName,
        InvalidEmail,
        InvalidPassword,
        InvalidAnswer,
        InvalidQuestion,
        UserRejected,


        PasswordUpdateSuccess,
        PasswordUpdateFail,
       
        //Entity
        EntityNotExists

    }
}
