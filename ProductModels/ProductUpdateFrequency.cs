
namespace Models.Shop
{
    public enum ProductUpdateFrequency
    {
        //Do not update 
        None=0,

        //Send a single update
        Notify,

        //Update every time
        Update
    }   
}

