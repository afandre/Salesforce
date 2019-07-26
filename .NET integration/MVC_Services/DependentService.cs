using SFLicenseRequest_MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SFLicenseRequest_MVC.Services
{
    public class DependentService
    {
        /*
         * dependent picklists in salesforce are determined by a base64 string that needs to be decoded into 
         * a binary string that binary string contains the position of the dependent items.
         * Compare the binary string to the ordered list of dependents as positions in the list. 
         * Example: in a list of "a, b, c, d, e, f" the binary string 101100 would include "a, c, d"
         */
        public static DependentService init()
        {
            return new DependentService();
        }
        /*A little internal terminology
        "functional" - the group that the subordinate is dependent on is called the
        "subordinate" - the picklists that are determined
        */
        public async Task<string> GetDependentsAsync(string functionalGroup)
        {
            /*
            Start by getting of the values from the REST api
            REST class is holding all the connections to Salesforce
            our functional group is "Group__c" and the dependent is "User_Role_c"
            */
            REST restService = new REST();
            var functGroups = await GetFunctionalsWithOrder("Group__c");
            var roles = await restService.GetSearchFilter("User_Role__c");
            List<DependentPicklist> dpList = new List<DependentPicklist>();

            // first loop through the functional group and append your values to the list of objects
            foreach (var group in functGroups)
            {
                DependentPicklist dp = new DependentPicklist();
                dp.Index = int.Parse(group.Key);
                dp.Controller = group.Value;
                dpList.Add(dp);
            }
            // then generate a new list of decoded values for each of the dependent 
            List<DecodedValidFor> decodedValueForList = new List<DecodedValidFor>();
            foreach (var role in roles)
            {
                DecodedValidFor dcvf = new DecodedValidFor();
                dcvf.Name = role.Key;
                dcvf.Decoded = base64ToBits(role.Value);
                decodedValueForList.Add(dcvf);
            }
            //Generate a dependent list based on the group you want to get dependents for
            DependentPicklist functionalDependentPicklist = dpList.SingleOrDefault(dp => dp.Controller == functionalGroup);
            //generate a dependent list and loop through the decoded values and find the one based on the functional and add it to the dependent list
            List<string> dependentList = new List<string>();
            foreach (var dcvf in decodedValueForList)
            {
                for (int i = 0; i < dcvf.Decoded.Length; i++)
                {
                    if (dcvf.Decoded[i] == '1')
                    {
                        if (i == functionalDependentPicklist.Index)
                        {
                            dependentList.Add(dcvf.Name);
                        }

                    }

                }

            }
            
            functionalDependentPicklist.Dependents = dependentList;
            // serlialize the list and return it 
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            return serializer.Serialize(functionalDependentPicklist.Dependents);
        }
        public async Task<List<DropDownObject>> GetFunctionalsWithOrder(string functGroup)
        {
            //get the functional groups in order
            //the order is how you determine the dependents
            REST restService = new REST();
            var functGroups = await restService.GetSearchFilter(functGroup);
            List<DropDownObject> FunctionalGroupDropdownWithOrder = new List<DropDownObject>();

            int idx = 0;
            foreach (var group in functGroups)
            {
                DropDownObject ddo = new DropDownObject();
                ddo.Key = idx.ToString();
                ddo.Value = group.Value;
                FunctionalGroupDropdownWithOrder.Add(ddo);
                idx++;

            }
            return FunctionalGroupDropdownWithOrder;

        }
        public static string base64ToBits(string validFor)
        {
            /*
            validFor is a value set on the dependent picklist showing the binary order of the dependencies
            you have to decode it from the hexidecimal value
             */
            string validForBits = "";
            string base64Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            for (int i = 0; i < validFor.Length; i++)
            {
                string thisChar = validFor.Substring(i, 1);
                int val = base64Chars.IndexOf(thisChar);
                string bits = Convert.ToString(val, 2).PadLeft(6, '0');
                validForBits += bits;
            }
            return validForBits;
        }
    }
}
