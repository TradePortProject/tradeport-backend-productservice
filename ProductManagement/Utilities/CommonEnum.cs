using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProductManagement.Utilities
{

    // --Fashion 1--
    //--Furniture 2--
    //--Home & Garden 3--
    //--Health & Beauty 4--
    //--Computer & Office 5--
    public enum Category
    {
        [Description("Fashion")]
        Fashion = 1,
        [Description("Furniture")]
        Furniture = 2,
        [Description("Home & Garden")]
        HomeAndGarden = 3,
        [Description("Health & Beauty")]
        HealthAndBeauty = 4,
        [Description("Computer & Office")]
        ComputerAndOffice = 5
    }

}
