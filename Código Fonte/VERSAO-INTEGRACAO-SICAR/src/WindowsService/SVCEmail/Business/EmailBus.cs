using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.Criptografia.Business;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.EtramiteX.WindowsService.SVCEmail.Data;
using Tecnomapas.EtramiteX.WindowsService.SVCEmail.Entities;
using Tecnomapas.EtramiteX.WindowsService.Utilitarios;

namespace Tecnomapas.EtramiteX.WindowsService.SVCEmail
{
	class EmailBus : ServicoTimerBase
	{
		private EmailDa _da = new EmailDa();
		private ConfiguracaoEmail _config;

		public void Teste()
		{
			Executar();
		}

		protected override void Executar()
		{
			try
			{
				_config = _da.ObterConfiguracao();

				if (_config == null)
				{
					throw new Exception("Não há linha na configuração do email. [cnf_email]");
				}

				Email email = _da.ObterEmailDeEnvio(_config.NumeroTentativaEnvio);

				while (email != null)
				{
					try
					{
						Enviar(email);
					}
					catch (Exception exc)
					{
						Log.GerarLog(exc);
					}
					finally
					{
						if (email.Situacao != 2 && email.NumTentativas >= _config.NumeroTentativaEnvio)
						{
							_da.AlterarSituacao(email.Id, 3);
						}
						else
						{
							_da.FimDaFila(email.Id, _config);
						}
					}

					email = _da.ObterEmailDeEnvio(_config.NumeroTentativaEnvio);
				}
			}
			catch (Exception exc)
			{
				Log.GerarLog(exc);
			}
		}

		private void Enviar(Email email)
		{

			if (email.Anexos != null && email.Anexos.Count > 0)
			{
				ArquivoBus arquivoBus = new ArquivoBus(eExecutorTipo.Interno);

				for (int i = 0; i < email.Anexos.Count; i++)
				{
					if (email.Anexos[i].Id.GetValueOrDefault() <= 0)
					{
						continue;
					}
					email.Anexos[i] = arquivoBus.Obter(email.Anexos[i].Id.GetValueOrDefault());
				}
			}

			if (_config.Porta == 465)
			{
				//COM
				EnviarCDO(email);
			}
			else
			{
				EnviarDotNet(email);
			}

			email.Situacao = 2;
			_da.AlterarSituacao(email.Id, email.Situacao);
		}

		private void EnviarDotNet(Email email)
		{
			String msgSenha = Decrypt.Executar(_config.SmtpSenha);

			MailMessage message = new MailMessage(_config.Remetente, email.Destinatario.Replace(';', ','));

			message.Subject = email.Assunto;
			message.Body = email.Texto;
			message.IsBodyHtml = true;
			message.BodyEncoding = System.Text.UTF8Encoding.UTF8;

			if (email.Anexos != null && email.Anexos.Count > 0)
			{
				foreach (var arquivo in email.Anexos.Where(x => x.Id.HasValue))
				{
					message.Attachments.Add(new Attachment(arquivo.Buffer, arquivo.Nome));
				}
			}

			NetworkCredential net = new NetworkCredential(_config.SmtpUser, msgSenha);

			SmtpClient client = new SmtpClient(_config.SmtpServer, _config.Porta);
			client.EnableSsl = _config.EnableSsl;
			client.Credentials = net;
			client.Send(message);
		}

		private void EnviarCDO(Email email)
		{
			String msgSenha = Decrypt.Executar(_config.SmtpSenha);

			CDO.Message message = new CDO.Message();
			CDO.IConfiguration configuration = message.Configuration;
			ADODB.Fields fields = configuration.Fields;
			ADODB.Field field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"];
			field.Value = _config.SmtpServer;
			field = fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"];
			field.Value = _config.Porta;
			field = fields["http://schemas.microsoft.com/cdo/configuration/sendusing"];
			field.Value = CDO.CdoSendUsing.cdoSendUsingPort;
			field = fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"];
			field.Value = CDO.CdoProtocolsAuthentication.cdoBasic;
			field = fields["http://schemas.microsoft.com/cdo/configuration/sendusername"];
			field.Value = _config.SmtpUser;
			field = fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"];
			field.Value = msgSenha;
			field = fields["http://schemas.microsoft.com/cdo/configuration/smtpusessl"];
			field.Value = _config.EnableSsl ? "true" : "false";
			fields.Update();

			List<string> lstArquivos = CopyTempPath(email);
			lstArquivos.ForEach(x => message.AddAttachment(x));

			message.From = _config.Remetente;
			message.To = email.Destinatario.Replace(';', ',');
			message.Subject = email.Assunto;
			message.HTMLBody = email.Texto;

			try
			{
				message.Send();
			}
			finally
			{
				lstArquivos.ForEach(x => System.IO.File.Delete(x));
				string path = GerarDirTemp(email.Id);
			}
		}

		private string GerarDirTemp(int id)
		{
			return String.Format("{0}ServicoIDAFEmail\\{1}\\", System.IO.Path.GetTempPath(), id);
		}

		private List<string> CopyTempPath(Email email)
		{
			List<string> lstArquivos = new List<string>();

			string path = GerarDirTemp(email.Id);

			if (System.IO.Directory.Exists(path))
			{
				System.IO.Directory.Delete(path, true);
			}
			System.IO.Directory.CreateDirectory(path);

			email.Anexos.ForEach(arquivo =>
			{

				String nomeArquivo = String.Concat(path, "\\",
					System.IO.Path.GetFileNameWithoutExtension(arquivo.Nome),
					System.IO.Path.GetFileNameWithoutExtension(arquivo.TemporarioNome),
					arquivo.Id,
					System.IO.Path.GetExtension(arquivo.Nome));
				lstArquivos.Add(nomeArquivo);

				System.IO.FileStream fsTemp = System.IO.File.Create(nomeArquivo);
				try
				{
					arquivo.Buffer.CopyTo(fsTemp);
					fsTemp.Flush();
				}
				finally
				{
					if (arquivo.Buffer != null)
					{
						arquivo.Buffer.Close();
						arquivo.Buffer.Dispose();
					}

					if (fsTemp != null)
					{
						fsTemp.Close();
						fsTemp.Dispose();
					}
				}
			});

			return lstArquivos;
		}

		public EmailBus(string esquema = null)
		{
			if (esquema != null)
			{
				_da = new EmailDa(esquema);
			}
		}
	}
}