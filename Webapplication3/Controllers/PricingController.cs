using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net;
using Pricing;
using Microsoft.Extensions.Logging;

namespace Pricing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Pricing : ControllerBase
    {
        static List<String> pricingDetailsBillTo = new List<String>();
        static List<String> pricingDetailsBSTo = new List<String>();
        static List<String> pricingDetailsBSERPNo = new List<String>();
        PricingObj pricing = new PricingObj();
        HttpResponseMessage response;
        SqlConnection connection = new SqlConnection("Data Source= CONECSTGWIS1; Initial Catalog = EcomETL; User ID = EcomUser ; Password = userecom; Persist Security Info = True");
        MemoryCache cache = new MemoryCache();
        private readonly ILogger<Pricing> _logger;
        private readonly IMemoryCache _memoryCache;

        /* GetPricingDataBy_BillTo with BillTo parameter */

        [HttpGet("{BillTo}")]
        public async Task<List<string>> GetPricingData(string BillTo)
        {
 
            try
                {
                if (BillTo.Length <= 8)
                {
                    pricingDetailsBillTo.Clear();
                    pricingDetailsBillTo.Add("Input string was not in the correct format");
                    InsertintoLog("200", "Input string was not in the correct format");
                    return pricingDetailsBillTo;
                }
                var matches = Regex.Matches(BillTo, @"\d+");
                if ((matches.Count) == 0)
                {
                    pricingDetailsBillTo.Clear();
                    pricingDetailsBillTo.Add("Input string was not in the correct format");
                    InsertintoLog("300", "Input must be numeric");
                    return pricingDetailsBillTo;
                }

                if (pricingDetailsBillTo.Count > 1)
                {
                    return pricingDetailsBillTo;
                }

                using (SqlCommand cmd = new SqlCommand("usp_ECOMETL_GetPricingDataByBillTo", connection) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                    
                    connection.Open();
                        string sBillTo = string.Empty;
                        int iBillTo = 0;
                        var bmatches = Regex.Matches(BillTo, @"\d+");
                        foreach (var match in matches)
                        {
                            sBillTo += match;
                        }
                        iBillTo = int.Parse(sBillTo);
                        pricing.billTo = iBillTo;

                        cmd.Parameters.Add("@BillTo", SqlDbType.BigInt);
                        // Set the input parameter value.
                        cmd.Parameters["@BillTo"].Value = iBillTo;

                        SqlDataReader reader = cmd.ExecuteReader();
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        pricingDetailsBillTo.Add(Convert.ToString(reader.GetValue(i)));
                                    }
                                }
                           cache.MemoryCaching();
                        }
                            else

                            {
                                pricingDetailsBillTo.Clear();
                                pricingDetailsBillTo.Add("No Rows in the Database");
                                InsertintoLog("400", "No rows in the Database");
                            return pricingDetailsBillTo;
                            }
                        }
                        reader.Close();
                        connection.Close();
                    
                    }
                  
                }
                catch (HttpRequestException ex)
                {

                    InsertintoLog(ex.ToString(), ex.Message);
                }
            return pricingDetailsBillTo;
        }


        ///* GetPricingDataBy_BillTo with BillTo, ShipTo parameter */
        [HttpGet("{BillTo}/{ShipTo}")]
        public async Task<List<string>> GetPricingDataBy_BillToShipTo(string BillTo, string ShipTo)
        {
            try
            {
                if (ShipTo.Length <= 8)
                {
                    pricingDetailsBSTo.Clear();
                    pricingDetailsBSTo.Add("Input string was not in the correct format");
                    InsertintoLog("200", "Input string was not in the correct format");
                    return pricingDetailsBSTo;
                }

               // MemoryCachingInitialize();

                int iBillTo = 0;
                string sBillTo = string.Empty;
                var matches = Regex.Matches(BillTo, @"\d+");
                foreach (var match in matches)
                {
                    sBillTo += match;
                }
                iBillTo = int.Parse(sBillTo);
                pricing.billTo = iBillTo;

                string sShipTo = string.Empty;
                int iShipTo = 0;
                var smatches = Regex.Matches(ShipTo, @"\d+");
                if ((smatches.Count) == 0)
                {
                    pricingDetailsBSTo.Clear();
                    pricingDetailsBSTo.Add("Input string must be a numeric");
                    InsertintoLog("300", "Input string is not a numeric");
                    return pricingDetailsBSTo;
                }
                foreach (var smatch in smatches)
                {
                    sShipTo += smatch;
                }
                iShipTo = int.Parse(sShipTo);
                pricing.shipTo = iShipTo;
                pricingDetailsBSTo.Clear();
                using (SqlCommand cmd = new SqlCommand("usp_ECOMETL_GetPricingDataByBillToShipTo", connection) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    connection.Open();
                    {
                        cmd.Parameters.Add("@BillTo", SqlDbType.BigInt);
                        cmd.Parameters["@BillTo"].Value = pricing.billTo;
                        cmd.Parameters.Add("@ShipTo", SqlDbType.BigInt);
                        cmd.Parameters["@ShipTo"].Value = pricing.shipTo;

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    pricingDetailsBSTo.Add(Convert.ToString(reader.GetValue(i)));
                                }
                            }
                        }
                        else
                        {
                            //  InsertintoLog("200", "No Rows in DB");
                            pricingDetailsBSTo.Clear();
                            pricingDetailsBSTo.Add("No Rows in the DB");
                            InsertintoLog("400", "No rows in the database");
                            return pricingDetailsBSTo;
                        }
                        reader.Close();
                        connection.Close();
                        

                    }
                }
            }
            catch (HttpRequestException ex)
            {
                InsertintoLog(ex.ToString(), ex.Message);
            }
            return pricingDetailsBSTo;

        }


        /* GetPricingDataBy_BillTo with BillTo, ShipTo, ERPNo */
        [HttpGet("{BillTo}/{ShipTo}/{ERPNumber}")]
        public async Task<List<string>> GetPricingDataBy_BSERPNo(string BillTo, string ShipTo, string ERPNumber)
        {
            try
            {
                if (ERPNumber.Length <= 8)
                {
                    pricingDetailsBSERPNo.Clear();
                    pricingDetailsBSERPNo.Add("Input string was not in the correct format");
                    InsertintoLog("200", "Input string was not in the correct format");
                    return pricingDetailsBSERPNo;
                }
              //  MemoryCachingInitialize();
                string sBillTo = string.Empty;
                int iBillTo = 0;
                var matches = Regex.Matches(BillTo, @"\d+");
                foreach (var match in matches)
                {
                    sBillTo += match;
                }
                iBillTo = int.Parse(sBillTo);
                pricing.billTo = iBillTo;

                string sShipTo = string.Empty;
                int iShipTo = 0;
                var smatches = Regex.Matches(ShipTo, @"\d+");
                foreach (var smatch in smatches)
                {
                    sShipTo += smatch;
                }
                iShipTo = int.Parse(sShipTo);
                pricing.shipTo = iShipTo;

                string sERPNumber = string.Empty;
                int iERPNumber = 0;
                var ematches = Regex.Matches(ERPNumber, @"\d+");
                if ((ematches.Count) == 0)
                {
                    pricingDetailsBSERPNo.Clear();
                    pricingDetailsBSERPNo.Add("Input string was not in the correct format");
                    InsertintoLog("300", "Input string must be a numeric");
                    return pricingDetailsBSERPNo;
                }
                foreach (var ematch in ematches)
                {
                    sERPNumber += ematch;
                }
                iERPNumber = int.Parse(sERPNumber);
                pricing.prodErPNo = iERPNumber;


                using (SqlCommand cmd = new SqlCommand("usp_ECOMETL_GetPricingDataByBSERPNo", connection) { CommandType = System.Data.CommandType.StoredProcedure })
                {
                    connection.Open();
                    {
                        cmd.Parameters.Add("@BillTo", SqlDbType.BigInt);
                        cmd.Parameters["@BillTo"].Value = pricing.billTo;
                        cmd.Parameters.Add("@ShipTo", SqlDbType.BigInt);
                        cmd.Parameters["@ShipTo"].Value = pricing.shipTo;
                        cmd.Parameters.Add("@ERPNumber", SqlDbType.BigInt);
                        cmd.Parameters["@ERPNumber"].Value = pricing.prodErPNo;

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    pricingDetailsBSERPNo.Add(Convert.ToString(reader.GetValue(i)));
                                }
                            }
                        }
                        else
                        {
                            pricingDetailsBSTo.Clear();
                            pricingDetailsBSERPNo.Add("No Rows in the DB");
                            InsertintoLog("400", "No rows in the Database");

                            return pricingDetailsBSERPNo;
                        }
                        reader.Close();
                        connection.Close();
                        

                    }
                }
            }
            catch (HttpRequestException ex)
            {
                InsertintoLog(ex.ToString(), ex.Message);
            }
            return pricingDetailsBSERPNo;

        }

        public int InsertintoLog(string Errorcode, string ErrorDescription)
        {
            int DataLogged = 0;
            SqlConnection connection = new SqlConnection("Data Source= CONECSTGWIS1; Initial Catalog = EcomETL; User ID = EcomUser ; Password = userecom; Persist Security Info = True");

            string SQLquery = "INSERT INTO APIErrorLog (ErrorCode, ErrorDescription, DateTime) VALUES(@ErrorCode, @ErrorDescription, @DTime)";
            SqlCommand SQLcmd = new SqlCommand(SQLquery, connection);
            connection.Open();
            if (Errorcode != null)
        
            {
                DataLogged = 1;

                SQLcmd.Parameters.AddWithValue("@ErrorCode", Errorcode);
                SQLcmd.Parameters.AddWithValue("@ErrorDescription", ErrorDescription);
                SQLcmd.Parameters.AddWithValue("@DTime", DateTime.Now);
                SQLcmd.ExecuteNonQuery();
            }
            return DataLogged;
        }

        



    }
}
