using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportGenerator
{
    class QuarterlyIncomeReport
    {
        static void Main(string[] args)
        {
            // create a new instance of the class
            QuarterlyIncomeReport report = new QuarterlyIncomeReport();

            // call the GenerateSalesData method
            SalesData[] salesData = report.GenerateSalesData();
            
            // call the QuarterlySalesReport method
            report.QuarterlySalesReport(salesData);
        }

        //
        /* public struct SalesData includes the following fields: date sold, department name, product ID, quantity sold, unit price */
        // alterado segundo exercício proposto
        public struct SalesData
        {
            public DateOnly dateSold;
            public string departmentName;
            public string productID;
            public int quantitySold;
            public double unitPrice;
            public double baseCost;
            public int volumeDiscount;
        }
        /// <summary>
        /// Provides department names and their abbreviations for product categorization.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="DepartmentNames"/> contains the full names of the departments.
        /// <see cref="DepartmentAbbr"/> contains the corresponding abbreviations.
        /// </para>
        /// </remarks>
        public struct ProdDepartments
        {
            public static readonly string[] DepartmentNames = new string[]
            {
                "Menswear",
                "Womenswear",
                "Kidswear",
                "Footwear",
                "Accessories",
                "Sportswear",
                "Outerwear",
                "Lingerie"
            };

            public static readonly string[] DepartmentAbbr = new string[]
            {
                "MENS",
                "WMNS",
                "KIDS",
                "FTWR",
                "ACCS",
                "SPRT",
                "OTWR",
                "LING"
            };
        }
        /* Atualiza SalesData para incluir baseCost (double), volumeDiscount (int) e altera productID para string */

        public struct ManufacturingSites
        {
            public static readonly string[] ManSites = new string[]
            {
                "BR1", "BR2", "BR3", // Brasil
                "US1", "US2", "US3", // Estados Unidos
                "DE1", "DE2", "DE3", "DE4" // Alemanha
            };
        }
        /* the GenerateSalesData method returns 1000 SalesData records. It assigns random values to each field of the data structure */
        public SalesData[] GenerateSalesData()
        {
            SalesData[] salesData = new SalesData[1000];
            Random random = new Random();

            for (int i = 0; i < 1000; i++)
            {
                salesData[i].dateSold = new DateOnly(2023, random.Next(1, 13), random.Next(1, 29));
                salesData[i].departmentName = ProdDepartments.DepartmentNames[random.Next(ProdDepartments.DepartmentNames.Length)];

                int indexOfDept = Array.IndexOf(ProdDepartments.DepartmentNames, salesData[i].departmentName);
                string deptAbb = ProdDepartments.DepartmentAbbr[indexOfDept];
                string firstDigit = (indexOfDept + 1).ToString();
                string nextTwoDigits = random.Next(1, 100).ToString("D2");
                string[] sizes = { "XS", "S", "M", "L", "XL" };
                string sizeCode = sizes[random.Next(sizes.Length)];
                string[] colorCodes = { "BK", "BL", "GR", "RD", "YL", "OR", "WT", "GY" };
                string colorCode = colorCodes[random.Next(colorCodes.Length)];
                string manufacturingSite = ManufacturingSites.ManSites[random.Next(ManufacturingSites.ManSites.Length)];

                salesData[i].productID = $"{deptAbb}-{firstDigit}-{nextTwoDigits}-{sizeCode}-{colorCode}-{manufacturingSite}";
                salesData[i].quantitySold = random.Next(1, 101);
                salesData[i].unitPrice = random.Next(25, 300) + random.NextDouble();
                double discountPercent = random.Next(5, 21); // 5 to 20 inclusive
                salesData[i].baseCost = salesData[i].unitPrice * (1 - discountPercent / 100.0);
                salesData[i].volumeDiscount = (int)(salesData[i].quantitySold * 0.10);
                // Ensure that the base cost is not negative
                
            }

            return salesData;
        }
        // Atualiza o relatório para exibir os trimestres em ordem e formata a moeda conforme a cultura local
        public void QuarterlySalesReport(SalesData[] salesData)
        {
            var quarterlySales = new Dictionary<string, double>();
            var quarterlyProfit = new Dictionary<string, double>();

            // Nested dictionary: quarter -> department -> sales/profit
            var deptQuarterlySales = new Dictionary<string, Dictionary<string, double>>();
            var deptQuarterlyProfit = new Dictionary<string, Dictionary<string, double>>();

            foreach (SalesData data in salesData)
            {
            string quarter = GetQuarter(data.dateSold.Month);
            string dept = data.departmentName;
            double totalSales = data.quantitySold * data.unitPrice;
            double totalCost = data.quantitySold * data.baseCost;
            double profit = totalSales - totalCost;

            // Overall quarterly
            if (quarterlySales.ContainsKey(quarter))
            {
                quarterlySales[quarter] += totalSales;
                quarterlyProfit[quarter] += profit;
            }
            else
            {
                quarterlySales[quarter] = totalSales;
                quarterlyProfit[quarter] = profit;
            }

            // Department quarterly
            if (!deptQuarterlySales.ContainsKey(quarter))
            {
                deptQuarterlySales[quarter] = new Dictionary<string, double>();
                deptQuarterlyProfit[quarter] = new Dictionary<string, double>();
            }
            if (deptQuarterlySales[quarter].ContainsKey(dept))
            {
                deptQuarterlySales[quarter][dept] += totalSales;
                deptQuarterlyProfit[quarter][dept] += profit;
            }
            else
            {
                deptQuarterlySales[quarter][dept] = totalSales;
                deptQuarterlyProfit[quarter][dept] = profit;
            }
            }

            string[] quartersOrder = { "Q1", "Q2", "Q3", "Q4" };
            Console.WriteLine("Quarterly Sales Report");
            Console.WriteLine("----------------------");
            Console.WriteLine("{0,-4} {1,15} {2,15} {3,15}", "QTR", "Sales", "Profit", "Profit %");
            foreach (string quarter in quartersOrder)
            {
            if (quarterlySales.ContainsKey(quarter))
            {
                double sales = quarterlySales[quarter];
                double profit = quarterlyProfit[quarter];
                double profitPercent = sales > 0 ? (profit / sales) * 100 : 0;
                Console.WriteLine("{0,-4} {1,15:C} {2,15:C} {3,14:N2}%", quarter, sales, profit, profitPercent);

                // Department breakdown
                if (deptQuarterlySales.ContainsKey(quarter))
                {
                foreach (var dept in deptQuarterlySales[quarter].Keys.OrderBy(d => d))
                {
                    double deptSales = deptQuarterlySales[quarter][dept];
                    double deptProfit = deptQuarterlyProfit[quarter][dept];
                    double deptProfitPercent = deptSales > 0 ? (deptProfit / deptSales) * 100 : 0;
                    Console.WriteLine("   Departament - {0,-12} {1,11:C} {2,15:C} {3,14:N2}%", dept, deptSales, deptProfit, deptProfitPercent);
                }
                }
            }
            }
        }
        
       

        public string GetQuarter(int month)
        {
            if (month >= 1 && month <= 3)
            {
                return "Q1";
            }
            else if (month >= 4 && month <= 6)
            {
                return "Q2";
            }
            else if (month >= 7 && month <= 9)
            {
                return "Q3";
            }
            else
            {
                return "Q4";
            }
        }
    }
}
