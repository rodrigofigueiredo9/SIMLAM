using System;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business
{
	public class HabilitacaoEmissaoPTVValidar
	{
		#region Propriedades

		private HabilitacaoEmissaoPTVDa _da = new HabilitacaoEmissaoPTVDa();
		private static GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		#endregion

		internal bool Salvar(HabilitacaoEmissaoPTV habilitacao)
		{
			if (habilitacao.Funcionario.Id <= 0)
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.VerificarCPFObrigatorio);
				return false;
			}

			if (string.IsNullOrEmpty(habilitacao.NumeroHabilitacao))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.NumeroHabilitacaoObrigatorio);
			}
			else
			{
				if (habilitacao.NumeroHabilitacao.Length != 8)
				{
					Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.NumeroHabilitacaoInvalido);
				}
				else if (_da.ExisteNumeroHabilitacao(habilitacao.NumeroHabilitacao, habilitacao.Funcionario.Id))
				{
					Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.NumeroHabilitacaoJaExiste);
				}
			}

			if (string.IsNullOrEmpty(habilitacao.RG))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.RGObrigatorio);
			}

			if (string.IsNullOrEmpty(habilitacao.NumeroMatricula))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.NumberoMatriculaObrigatorio);
			}
			else if (habilitacao.NumeroMatricula.Length != 7)
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.NumberoMatriculaInvalido);
			}

			if (habilitacao.EstadoRegistro <= 0)
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.UFHabilitacaoObrigatorio);
			}
			else
			{
				ListaBus _busLista = new ListaBus();
				Estado estado = _busLista.Estados.SingleOrDefault(x => String.Equals(x.Texto, _configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault), StringComparison.InvariantCultureIgnoreCase)) ?? new Estado();

				if (habilitacao.EstadoRegistro != estado.Id)
				{
					if (string.IsNullOrEmpty(habilitacao.NumeroVistoCREA))
					{
						Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.NumeroVistoCREAObrigatorio);
					}
				}
				else
				{
					if (string.IsNullOrEmpty(habilitacao.NumeroCREA))
					{
						Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.NumeroCREAObrigatorio);
					}
				}
			}

			if (string.IsNullOrEmpty(habilitacao.TelefoneResidencial) && string.IsNullOrEmpty(habilitacao.TelefoneCelular) && string.IsNullOrEmpty(habilitacao.TelefoneComercial))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.MeioContatoObrigatorio);
			}

			if (string.IsNullOrEmpty(habilitacao.Endereco.Logradouro))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.LogradouroObrigatorio);
			}

			if (string.IsNullOrEmpty(habilitacao.Endereco.Bairro))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.BairroObrigatorio);
			}

			if (habilitacao.Endereco.EstadoId <= 0)
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.UFObrigatorio);
			}

			if (habilitacao.Endereco.MunicipioId <= 0)
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.MunicipioObrigatorio);
			}

			if (string.IsNullOrEmpty(habilitacao.Endereco.Numero))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.NumeroObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool VerificarCPF(string cpf)
		{
			if (string.IsNullOrEmpty(cpf))
			{
				Validacao.Add(Mensagem.Pessoa.CpfObrigatorio);
				return false;
			}

			if (!ValidacoesGenericasBus.Cpf(cpf))
			{
				Validacao.Add(Mensagem.Pessoa.CpfInvalido);
				return false;
			}

			if (!_da.CPFAssociadoFuncionario(cpf))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.CPFNaoAssociadoAFuncionario);
				return false;
			}

			if (!_da.FuncionarioDesabilitado(cpf))
			{
				Validacao.Add(Mensagem.HabilitacaoEmissaoPTV.FuncionarioHabilitado);
				return false;
			}

			return true;
		}		
	}
}