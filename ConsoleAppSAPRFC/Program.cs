using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSAPRFC
{
    class Program
    {
        static void Main(string[] args)
        {
            var dictConfig = ReadConfig(@"D:\Cust_SAP_Parameters.ini");
            //UtilSAP usap = nullll new UtilSAP();

            RfcConfigParameters config = new RfcConfigParameters();
            config.Add(RfcConfigParameters.Name, dictConfig["Name"]);
            config.Add(RfcConfigParameters.AppServerHost, dictConfig["AppServerHost"]);
            config.Add(RfcConfigParameters.SystemNumber, dictConfig["SystemNumber"]);
            config.Add(RfcConfigParameters.User, dictConfig["User"]);
            config.Add(RfcConfigParameters.Password, dictConfig["Password"]);
            config.Add(RfcConfigParameters.Client, dictConfig["Client"]);
            config.Add(RfcConfigParameters.Language, dictConfig["Language"]);

            RfcDestination destination = RfcDestinationManager.GetDestination(config);
            RfcRepository repository = null;
            IRfcFunction functionRFC = null;
            repository = destination.Repository;
            functionRFC = repository.CreateFunction("ZES_RFC_SPFLI");

            //PARAMETROS IMPORT
            
            functionRFC.SetValue("IS_NAME", "Elvis");
            functionRFC.SetValue("IS_ROWS", "15");

            IRfcTable rangePersonal = functionRFC.GetTable("IT_COUNTRYTO");
            rangePersonal.Clear();
            rangePersonal.Append();//importante ponerlo sino nos lanzara error.
            rangePersonal.SetValue("SIGN", "I");
            rangePersonal.SetValue("OPTION", "EQ");
            rangePersonal.SetValue("LOW", "US");
            rangePersonal.SetValue("HIGH", "");
            functionRFC.SetValue("IT_COUNTRYTO", rangePersonal);

            try
            {

                //INVOKE
                functionRFC.Invoke(destination);

                //PARAMETROS EXPORT

                string resultado = functionRFC.GetValue("ES_MESSAGE").ToString();

                IRfcTable tData = functionRFC.GetTable("ET_DATA");

                // IRfcStructure mensaje = functionRFC.GetStructure("EW_MENSAJE");

                DataTable dtData = null;
                dtData = new DataTable();
                dtData = UtilSAP.toDataTable(tData, "Tabla de Datos");
                //UploadDataToDB(dtData);

                Console.WriteLine("CAMPO [RESULTADO]: " + resultado);
                Console.WriteLine("ESTRUCTURA [MENSAJE]: ");
                //Console.WriteLine("[IND_EXITO] = " + mensaje.GetString("IND_EXITO"));
                //Console.WriteLine("[MESSAGE] = " + mensaje.GetString("MESSAGE"));
                Console.WriteLine("TABLA PERSONAL:");
                foreach (DataRow fila in dtData.Rows)
                {
                    // Console.WriteLine("Client: " + fila[0].ToString());
                    Console.WriteLine("Mandante: " + fila[0].ToString());
                    Console.WriteLine("Airline Code: " + fila[1].ToString());
                    Console.WriteLine("Flight Connection Number: " + fila[2].ToString());
                    Console.WriteLine("Country Key (FROM): " + fila[3].ToString());
                    Console.WriteLine("Departure city: " + fila[4].ToString());
                    Console.WriteLine("Country Key (TO): " + fila[6].ToString());
                    Console.WriteLine("Arrival city: " + fila[7].ToString());
                    Console.WriteLine("");

                }
                Console.WriteLine();

                Console.ReadKey();

            }
            catch (RfcAbapException ex)
            {
                if (ex.Key == "CARR_NOT_FOUND")
                {
                    Console.WriteLine("Tabla no posee data para esos valores de entrada.");
                }
                else
                {
                    Console.WriteLine(ex.Message);
                }
                    
            }
        }


        static void UploadDataToDB(DataTable dt)
        {
            string strConnString = ConfigurationManager.ConnectionStrings["DBConn"].ToString();

            try
            {
                using (SqlBulkCopy sbc = new SqlBulkCopy(strConnString))
                {
                    sbc.BatchSize = 10000;
                    sbc.BulkCopyTimeout = 10000;
                    //Columnas desde SAP a DB
                    sbc.ColumnMappings.Add("ROWS_SAP", "ROWS_SQL"); // (campo_sap, campo_db)
                    sbc.ColumnMappings.Add("NOMBRE_SAP", "NOMBRE_SQL");

                    sbc.DestinationTableName = "TBL_DATA_SQL";// Tabla de BD
                    sbc.WriteToServer(dt);
                }

            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.ToString());
            }

        }

        public static Dictionary<string, string> ReadConfig(string txtFile)
        {
            var dict = new Dictionary<string, string>();
            string line;
            using (StreamReader sr = new StreamReader(txtFile))
            {
                while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                {
                    var tmp = line.Split('=');
                    dict.Add(tmp[0], tmp[1]);
                }

                return dict;
            }
        }



        //public static string GetValueDict(string key, Dictionary<string, string> dictionary)
        //{
        //    string value = "";
        //    foreach (var item in dictionary)
        //    {
        //        if (item.Key == key)
        //        {
        //            value = item.Value;
        //            break;
        //        }
        //    }
        //    return value;
        //}
    }
}

