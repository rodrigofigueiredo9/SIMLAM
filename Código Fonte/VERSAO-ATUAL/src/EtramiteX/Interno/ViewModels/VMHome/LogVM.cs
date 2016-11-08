using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Home;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMHome
{
    public class LogVM
    {
        public Log Log { get; set; }
        public string Source { get; set; }

        public LogVM(List<string> lstSource) 
        {
            Log = new Blocos.Entities.Home.Log();

            Log.LstSource = new List<SelectListItem>();

            lstSource.ForEach(source => {

                Log.LstSource.Add(new SelectListItem() { Text = source, Value = source });
            });

            Log.LstSource.Insert(0, new SelectListItem() {Selected=true, Text="***Todos***", Value="0" });
        }

        public LogVM()
        {
        }
    }
}