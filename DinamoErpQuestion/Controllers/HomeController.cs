using DinamoErpQuestion.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DinamoErpQuestion.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Tablo 1 Kayıtları
            var productionOperatiosRecord = new List<ProductionOperationsRecord>
            {
                new ProductionOperationsRecord { RecordNo = 1, Start = new DateTime(2020, 5, 23, 7, 30, 0), Finish = new DateTime(2020, 5, 23, 8, 30, 0), TotalTime = new TimeSpan(1, 0, 0), Status = "URETIM", ReasonForStoppingTheOperation = "" },
                new ProductionOperationsRecord { RecordNo = 2, Start = new DateTime(2020, 5, 23, 8, 30, 0), Finish = new DateTime(2020, 5, 23, 12, 0, 0), TotalTime = new TimeSpan(3, 30, 0), Status = "URETIM", ReasonForStoppingTheOperation = "" },
                new ProductionOperationsRecord { RecordNo = 3, Start = new DateTime(2020, 5, 23, 12, 0, 0), Finish = new DateTime(2020, 5, 23, 13, 0, 0), TotalTime = new TimeSpan(1, 0, 0), Status = "URETIM", ReasonForStoppingTheOperation = "" },
                new ProductionOperationsRecord { RecordNo = 4, Start = new DateTime(2020, 5, 23, 13, 0, 0), Finish = new DateTime(2020, 5, 23, 13, 45, 0), TotalTime = new TimeSpan(0, 45, 0), Status = "DURUS", ReasonForStoppingTheOperation = "ARIZA" },
                new ProductionOperationsRecord { RecordNo = 5, Start = new DateTime(2020, 5, 23, 13, 45, 0), Finish = new DateTime(2020, 5, 23, 17, 30, 0), TotalTime = new TimeSpan(3, 45, 0), Status = "URETIM", ReasonForStoppingTheOperation = "" }
            };

            //Tablo 2 Kayıtları
            var breakTimes = new List<(TimeSpan, TimeSpan, string)>
            {
                (new TimeSpan(10, 0, 0), new TimeSpan(10, 15, 0), "Çay Molası"),
                (new TimeSpan(12, 0, 0), new TimeSpan(12, 30, 0), "Yemek Molası"),
                (new TimeSpan(15, 0, 0), new TimeSpan(15, 15, 0), "Çay Molası")
            };

            var result = GenerateTable3(productionOperatiosRecord, breakTimes);
            //3 tablo olduğu için 1 ve 2. tabloları baz alarak tablo 3 oluşturmak yani sistem kurmama yardımcı olması için kullandım.
            return View(result);
        }

        private List<ProductionOperationsRecord> GenerateTable3(List<ProductionOperationsRecord> productionOperatiosRecord, List<(TimeSpan, TimeSpan, string)> breakTimes)
        {
            var result = new List<ProductionOperationsRecord>();

            foreach (var record in productionOperatiosRecord)
            {
                var downTime = TimeSpan.Zero;

                foreach (var breakTime in breakTimes)
                {
                    var breakStart = record.Start.Date.Add(breakTime.Item1);
                    var breakEnd = record.Start.Date.Add(breakTime.Item2);
                    //Molaların başlangıç ve bitiş zamanlarını alıyorum
                    if (record.Start < breakEnd && record.Finish > breakStart)
                    {
                        var start = record.Start < breakStart ? breakStart : record.Start;
                        var end = record.Finish > breakEnd ? breakEnd : record.Finish;
                        downTime += end - start;

                        if(record.Start != start) //Kodda 12.00 12.00 araligi bir hata aliyodum nedenini cozemedim o yuzden kosul koydum
                        {
                            //Araliga giren zamandaki ornegin 8.30-10.00 araligini yazdirmak icin
                            result.Add(new ProductionOperationsRecord
                            {

                                RecordNo = record.RecordNo,
                                Start = record.Start,
                                Finish = start,
                                TotalTime = start - record.Start,
                                Status = "URETIM",
                                ReasonForStoppingTheOperation = record.ReasonForStoppingTheOperation,

                            });

                        }
                        
                        // Duruş bilgisini ayrı bir kayıt olarak eklemek için
                        result.Add(new ProductionOperationsRecord
                        {
                            RecordNo = record.RecordNo,
                            Start = start,
                            Finish = end,
                            TotalTime = end - start,
                            Status = "DURUS",
                            ReasonForStoppingTheOperation = breakTime.Item3
                        });

                        // yeni kayıtlara kalan süreyi eklem 
                        if (end < record.Finish)
                        {
                            result.Add(new ProductionOperationsRecord
                            {
                                RecordNo = record.RecordNo,
                                Start = end,
                                Finish = record.Finish,
                                TotalTime = record.Finish - end,
                                Status = record.Status,
                                ReasonForStoppingTheOperation = record.ReasonForStoppingTheOperation
                            });
                        }
                        
                    }
                }

                if (downTime == TimeSpan.Zero)
                {
                    result.Add(record);
                }
            }

            return result.OrderBy(r => r.Start).ToList();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
