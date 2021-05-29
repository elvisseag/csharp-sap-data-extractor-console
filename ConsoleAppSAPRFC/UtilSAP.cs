using SAP.Middleware.Connector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppSAPRFC
{
    public class UtilSAP
    {

        public static DataTable toDataTable(IRfcTable tabRfc, string name)
        {
            DataTable dt = new DataTable(name);

            for (int index = 0; index < tabRfc.ElementCount; index++)
            {
                RfcElementMetadata mt = tabRfc.GetElementMetadata(index);
                dt.Columns.Add(mt.Name, GetDataType(mt.DataType));
            }

            foreach (IRfcStructure row in tabRfc)
            {
                DataRow dr = dt.NewRow();
                for (int index = 0; index < tabRfc.ElementCount; index++)
                {
                    RfcElementMetadata mt2 = tabRfc.GetElementMetadata(index);

                    switch (mt2.DataType)
                    {
                        case RfcDataType.DATE://para fecha
                            String fecha = row.GetString(mt2.Name);
                            if (!(fecha == "0000-00-00"))
                            {
                                dr[mt2.Name] = row.GetString(mt2.Name).Substring(0, 4) + "-" + row.GetString(mt2.Name).Substring(5, 2) + "-" + row.GetString(mt2.Name).Substring(8, 2);
                            }
                            break;// la fecha lo pasamos como String.
                        case RfcDataType.TIME://para hora
                            String hora = row.GetString(mt2.Name);
                            if (!(hora == "00:00:00"))
                            {
                                dr[mt2.Name] = row.GetString(mt2.Name).Substring(0, 2) + ":" + row.GetString(mt2.Name).Substring(3, 2) + ":" + row.GetString(mt2.Name).Substring(6, 2);
                            }
                            break;// la hora lo pasamos como String.
                        case RfcDataType.BCD://para decimal
                            dr[mt2.Name] = row.GetDecimal(mt2.Name);//aqui se le asigna el tipo de dato decimal a ese campo de la tabla.
                            break;
                        case RfcDataType.CHAR:
                            dr[mt2.Name] = row.GetString(mt2.Name);
                            break;
                        case RfcDataType.STRING:
                            dr[mt2.Name] = row.GetString(mt2.Name);
                            break;
                        case RfcDataType.INT2:
                            dr[mt2.Name] = row.GetInt(mt2.Name);
                            break;
                        case RfcDataType.INT4:
                            dr[mt2.Name] = row.GetInt(mt2.Name);
                            break;
                        case RfcDataType.INT8:
                            dr[mt2.Name] = row.GetInt(mt2.Name);
                            break;
                        case RfcDataType.FLOAT:
                            dr[mt2.Name] = row.GetDouble(mt2.Name);
                            break;
                        default:
                            dr[mt2.Name] = row.GetString(mt2.Name);
                            break;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;

        }//fin de funcion toDataTable


        private static Type GetDataType(RfcDataType rfcDataType)
        {
            switch (rfcDataType)
            {
                case RfcDataType.DATE:
                    return typeof(string);
                case RfcDataType.TIME:
                    return typeof(string);
                case RfcDataType.CHAR:
                    return typeof(string);
                case RfcDataType.STRING:
                    return typeof(string);
                case RfcDataType.BCD:
                    return typeof(decimal);
                case RfcDataType.INT2:
                    return typeof(int);
                case RfcDataType.INT4:
                    return typeof(int);
                case RfcDataType.INT8:
                    return typeof(int);
                case RfcDataType.FLOAT:
                    return typeof(double);
                default:
                    return typeof(string);
            }
        }//fin de funcion GetDataType

    }
}
