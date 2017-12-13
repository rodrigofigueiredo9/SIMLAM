using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceProcess;
using log4net;
using Quartz;
using Quartz.Impl;
using Tecnomapas.EtramiteX.Scheduler.jobs;

namespace Tecnomapas.EtramiteX.Scheduler
{
	
	public partial class SchedulerService : ServiceBase
	{
		private const string NomeServico = "Tecnomapas.EtramiteX.Scheduler";

		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		private static IScheduler _scheduler;
		private static readonly Dictionary<string, IJobDetail> JobDictionary = new Dictionary<string, IJobDetail>();

		public SchedulerService()
		{
			InitializeComponent();
		}

		public void OnDebug()
		{
			OnStart(null);
		}

		protected override void OnStart(string[] args)
		{
			Tecnomapas.EtramiteX.Scheduler.misc.LocalDB.ResetarFila();


			ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
			_scheduler = schedulerFactory.GetScheduler();
			_scheduler.Start();
			

			Log.Info(string.Format("Starting Windows Service: {0}", NomeServico));

			CreateJobs();

			//Log.Info("Criando Jobs");

            ScheduleJobIntegrar();

           // Log.Info("Integrando...");

			ScheduleJobs();

			//Log.Info("Agendando Jobs");
		}

		private static void CreateJobs()
		{
            var IntegracaoCar = JobBuilder.Create<IntegracaoCarJob>().WithIdentity("IntegracaoCarJob").Build();
            JobDictionary.Add("IntegracaoCarJob", IntegracaoCar);
            
            var gerarArquivoCar = JobBuilder.Create<GerarArquivoCarJob>().WithIdentity("GerarArquivoCarJob").Build();
            JobDictionary.Add("GerarArquivoCarJob", gerarArquivoCar);
			
		    var enviarArquivoCar = JobBuilder.Create<EnviarArquivoCarJob>().WithIdentity("EnviarArquivoCarJob").Build();
			JobDictionary.Add("EnviarArquivoCarJob", enviarArquivoCar);
            
            var ajustarStatusCar = JobBuilder.Create<AjustarStatusCarJob>().WithIdentity("AjustarStatusCarJob").Build();
            JobDictionary.Add("AjustarStatusCarJob", ajustarStatusCar);
            
            var consultarDua = JobBuilder.Create<ConsultarDUAJob>().WithIdentity("ConsultarDUAJob").Build();
            JobDictionary.Add("ConsultarDUAJob", consultarDua);
		}

		private static void ScheduleJobs()
		{
            _scheduler.ScheduleJob(JobDictionary["GerarArquivoCarJob"], CreateTrigger("A cada 15 Segundos"));
            _scheduler.ScheduleJob(JobDictionary["EnviarArquivoCarJob"], CreateTrigger("A cada 60 Segundos"));
            _scheduler.ScheduleJob(JobDictionary["AjustarStatusCarJob"], CreateTrigger("A cada 15 Segundos"));
            _scheduler.ScheduleJob(JobDictionary["ConsultarDUAJob"], CreateTrigger("A cada 5 Segundos"));
		}
        
        private static void ScheduleJobIntegrar()
        {
            _scheduler.ScheduleJob(JobDictionary["IntegracaoCarJob"], CreateTrigger("A cada 15 Segundos"));
        }

		private static ITrigger CreateTrigger(string key)
		{
			const int localTimeZone = -4;
			
			switch (key.ToUpper())
			{
				case "A CADA 1 SEGUNDO":
					return TriggerBuilder.Create()
						.WithDescription("A cada 1 Segundo")
						.WithSimpleSchedule(x => x
							.WithIntervalInSeconds(1)
							.RepeatForever()
							.WithMisfireHandlingInstructionNowWithExistingCount())
						.Build();

				case "A CADA 5 SEGUNDOS":
					return TriggerBuilder.Create()
						.WithDescription("A cada 5 Segundos")
						.WithSimpleSchedule(x => x
							.WithIntervalInSeconds(5)
							.RepeatForever()
							.WithMisfireHandlingInstructionNowWithExistingCount())
						.Build();

				case "A CADA 10 SEGUNDOS":
					return TriggerBuilder.Create()
						.WithDescription("A cada 10 Segundos")
						.WithSimpleSchedule(x => x
							.WithIntervalInSeconds(10)
							.RepeatForever()
							.WithMisfireHandlingInstructionNowWithExistingCount())
						.Build();

				case "A CADA 15 SEGUNDOS":
					return TriggerBuilder.Create()
						.WithDescription("A cada 15 Segundos")
						.WithSimpleSchedule(x => x
							.WithIntervalInSeconds(15)
							.RepeatForever()
							.WithMisfireHandlingInstructionNowWithExistingCount())
						.Build();

				case "A CADA 60 SEGUNDOS":
					return TriggerBuilder.Create()
						.WithDescription("A cada 60 Segundos")
						.WithSimpleSchedule(x => x
							.WithIntervalInSeconds(60)
							.RepeatForever()
							.WithMisfireHandlingInstructionNowWithExistingCount())
						.Build();

				case "A CADA 10 MINUTOS":
					return TriggerBuilder.Create()
						.WithDescription("A cada 10 Minutos")
						.WithSimpleSchedule(x => x
							.WithIntervalInMinutes(10)
							.RepeatForever()
							.WithMisfireHandlingInstructionNowWithExistingCount())
						.Build();

				case "A CADA 1 HORA":
					return TriggerBuilder.Create()
						.WithDescription("A cada 1 Hora")
						.WithSimpleSchedule(x => x
							.WithIntervalInHours(1)
							.RepeatForever()
							.WithMisfireHandlingInstructionNowWithExistingCount())
						.Build();

				case "TODO DIA AS 5AM (UTC-4)":
					return TriggerBuilder.Create()
						.WithDescription("Todo dia as 5AM (UTC-4)")
						.WithDailyTimeIntervalSchedule(x => x.
							WithIntervalInHours(24)
							.OnEveryDay()
							.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(5 - localTimeZone, 0))
							.InTimeZone(TimeZoneInfo.Utc))
						.Build();

				case "TODO DIA AS 6AM (UTC-4)":
					return TriggerBuilder.Create()
						.WithDescription("Todo dia as 6AM (UTC-4)")
						.WithDailyTimeIntervalSchedule(x => x.
							WithIntervalInHours(24)
							.OnEveryDay()
							.StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(6 - localTimeZone, 0))
							.InTimeZone(TimeZoneInfo.Utc))
						.Build();

				default:
					throw new Exception("Trigger not implemented:\"" + key + "\"");
			}

		}

		protected override void OnStop()
		{
			Log.Info(String.Format("Stopping Windows Service: {0}", NomeServico));
		}
	}
}
