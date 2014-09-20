namespace Annapolis.Shared.Model
{
    public enum EntityPermission
    {
        Read,
        Add,
        Update,
        Delete,
        Vote,
      
        UploadImage,
        UploadDocumentRead,
        UploadDocument,
   
    }

    public static class Extensions
    {
        public static bool IsDataChangePermission(this EntityPermission permission)
        {
            return permission == EntityPermission.Add || permission == EntityPermission.Update
                    || permission == EntityPermission.Delete || permission == EntityPermission.Vote;
        }
    }

}
