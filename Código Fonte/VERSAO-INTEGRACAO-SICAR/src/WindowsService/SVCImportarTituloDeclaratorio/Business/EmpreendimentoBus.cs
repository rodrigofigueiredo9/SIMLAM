using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
	public class EmpreendimentoBus
	{
        private EmpreendimentoDa _da = new EmpreendimentoDa();

		public EmpreendimentoBus() { }

		public Requerimento Importar(Requerimento requerimento, BancoDeDados bancoInterno, BancoDeDados bancoCredenciado)
		{
			int id = requerimento.Empreendimento.Id;

			EmpreendimentoCredenciadoBus bus = new EmpreendimentoCredenciadoBus();

			Empreendimento empCredenciado = bus.Obter(requerimento.Empreendimento.Id, bancoCredenciado);

			//Busca o empreendimento no banco do credenciado
			Empreendimento empInterno = null;

			empCredenciado.SelecaoTipo = requerimento.Empreendimento.SelecaoTipo;

			#region Empreendimento Cadastrado no Interno

			empCredenciado.Id = 0;

			if (empCredenciado.InternoId.GetValueOrDefault() > 0)
			{
				empInterno = Obter(empCredenciado.InternoId.GetValueOrDefault(), bancoInterno);

				if (empInterno != null && empInterno.Id > 0)
				{
					empCredenciado.Id = empInterno.Id;
					empCredenciado.Coordenada.Id = empInterno.Coordenada.Id;
				}
			}

			if (!string.IsNullOrEmpty(empCredenciado.CNPJ) && empCredenciado.Id <= 0)
			{
				empCredenciado.Id = _da.ObterId(empCredenciado.CNPJ, bancoInterno);
			}

			#endregion Empreendimento Cadastrado no Interno

			#region Responsáveis

			empCredenciado.Responsaveis.ForEach(r =>
			{
				r.Id = requerimento.Pessoas.Where(x => x.Id == r.Id).SingleOrDefault().InternoId.GetValueOrDefault();
			});

			ConfigurarResponsaveis(empCredenciado, bancoInterno, empInterno);

			#endregion Responsáveis

			#region Apagar ID's

			empCredenciado.Responsaveis.ForEach(r => { r.IdRelacionamento = 0; });
			empCredenciado.Enderecos.ForEach(r => { r.Id = 0; });
			empCredenciado.MeiosContatos.ForEach(r => { r.Id = 0; });
			empCredenciado.Coordenada.Datum.Sigla = ((_da.ObterDatuns().FirstOrDefault(x => x.Id == empCredenciado.Coordenada.Datum.Id)) ?? new Datum()).Texto;

			#endregion Apagar ID's

			if (empCredenciado.Id == 0)
			{
				_da.Criar(empCredenciado, bancoInterno);
			}
			else
			{
				_da.Editar(empCredenciado, bancoInterno);
			}

			empCredenciado.InternoId = empCredenciado.Id;
			requerimento.Empreendimento = empCredenciado;
			bus.SalvarInternoId(
				new Empreendimento()
				{
					Id = id,
					InternoId = empCredenciado.Id,
					InternoTid = empCredenciado.Tid,
					Codigo = empCredenciado.Codigo
				}, bancoCredenciado);

			return requerimento;
		}

	    public Empreendimento Obter(int id, BancoDeDados bancoInterno)
	    {
	        Empreendimento emp = _da.Obter(id, bancoInterno);

	        if (emp.Id == 0)
	        {
	            Validacao.Add(Mensagem.Empreendimento.Inexistente);
	        }

	        return emp;
	    }

	    public void ConfigurarResponsaveis(Empreendimento empCredenciado, BancoDeDados bancoInterno, Empreendimento empInterno = null)
		{
			if (empInterno == null)
			{
				if (empCredenciado.InternoId.GetValueOrDefault() > 0)
				{
					empInterno = Obter(empCredenciado.InternoId.GetValueOrDefault(), bancoInterno);
				}
				else
				{
					empInterno = new Empreendimento();
				}
			}

			CredenciadoPessoa credenciado = new CredenciadoBus().Obter(empCredenciado.CredenciadoId.GetValueOrDefault());

			List<Responsavel> responsaveis =
				empCredenciado.Responsaveis.Where(x => !empInterno.Responsaveis.Exists(y => y.CpfCnpj == x.CpfCnpj))
					.ToList();
			responsaveis.ForEach(r =>
			{
				r.CredenciadoUsuarioId = credenciado.Id;
				r.Origem = 1;
				r.OrigemTexto = "Inserido pelo Credenciado • Perfil: " + credenciado.TipoTexto;
			});

			empCredenciado.Responsaveis.ForEach(r =>
			{
				if (empInterno != null && empInterno.Id > 0)
				{
					Responsavel resp = empInterno.Responsaveis.FirstOrDefault(x => x.CpfCnpj == r.CpfCnpj);

					if (resp != null && r.Tipo != resp.Tipo)
					{
						r.CredenciadoUsuarioId = credenciado.Id;
						r.Origem = 1;
						r.OrigemTexto = "Alterado pelo Credenciado • Perfil: " + credenciado.TipoTexto;
					}
				}
			});

			if (empInterno != null && empInterno.Id > 0 && empCredenciado.InternoTid != null)
			{
				Empreendimento empHistorico = ObterHistorico(empCredenciado.InternoId.GetValueOrDefault(), empCredenciado.InternoTid, bancoInterno);

				responsaveis = empInterno.Responsaveis.Where(x =>
					!empCredenciado.Responsaveis.Exists(y => y.CpfCnpj == x.CpfCnpj) &&
					empHistorico.Responsaveis.Exists(y => y.CpfCnpj == x.CpfCnpj)).ToList();
				responsaveis.ForEach(r =>
				{
					r.CredenciadoUsuarioId = credenciado.Id;
					r.Origem = 1;
					r.OrigemTexto = "Removido pelo Credenciado • Perfil: " + credenciado.TipoTexto;
				});

				empCredenciado.Responsaveis.AddRange(responsaveis);

				empCredenciado.Responsaveis.Where(x => string.IsNullOrEmpty(x.OrigemTexto)).ToList().ForEach(r =>
				{
					Responsavel resp = empHistorico.Responsaveis.FirstOrDefault(x => x.CpfCnpj == r.CpfCnpj) ??
									   new Responsavel();
					r.OrigemTexto = resp.OrigemTexto;
				});
			}
		}

	    public Empreendimento ObterHistorico(int id, string tid, BancoDeDados bancoInterno)
	    {
	        return _da.ObterHistorico(id, tid, bancoInterno);
	    }
	}
}