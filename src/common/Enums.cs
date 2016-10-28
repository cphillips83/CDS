namespace CDS
{
    public enum ChangeEntryAction : byte
    {
        OK  = 0x00,
        Create = 0x01,
        Delete = 0x02,
        Replace = 0x03,

        //FileDelete,
        //FileCreate,
        //FileReplace,
        //DirectoryDelete,
        //DirectoryCreate
    }
}
