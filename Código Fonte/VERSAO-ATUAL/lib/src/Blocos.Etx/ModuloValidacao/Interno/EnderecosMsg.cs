

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static EnderecosMsg _enderecosMsg = new EnderecosMsg();
		public static EnderecosMsg Enderecos
		{
			get { return _enderecosMsg; }
		}
	}

	public class EnderecosMsg
	{
		public Mensagem EnderecoObrigatorio(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Endereço de {0} é obrigatório", nomeEndereco) };
		}

		public Mensagem EnderecoCepObrigatorio(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__Cep", objPaiNome, lstEnderecosNome, index), Texto = String.Format("CEP de {0} é obrigatório.", nomeEndereco) };
		}

		public Mensagem EnderecoCepInvalido(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__Cep", objPaiNome, lstEnderecosNome, index), Texto = String.Format("CEP de {0} é inválido.", nomeEndereco) };
		}

		public Mensagem EnderecoLogradouroObrigatorio(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__Logradouro", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Logradouro de {0} é obrigatório.", nomeEndereco) };
		}

		public Mensagem EnderecoBairroObrigatorio(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__Bairro", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Bairro de {0} é obrigatório.", nomeEndereco) };
		}

		public Mensagem EnderecoDistritoObrigatorio(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__DistritoLocalizacao", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Distrito de {0} é obrigatório.", nomeEndereco) };
		}

		public Mensagem EnderecoComplementoObrigatorio(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__Complemento", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Complemento de {0} é obrigatório.", nomeEndereco) };
		}
		
		public Mensagem EnderecoEstadoObrigatorio(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__EstadoId", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Estado de {0} é obrigatório.", nomeEndereco) };
		}

		public Mensagem EnderecoEstadoInvalido(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__EstadoId", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Estado de {0} é inválido.", nomeEndereco) };
		}

		public Mensagem EnderecoMunicipioObrigatorio(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__MunicipioId", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Municipio de {0} é obrigatório.", nomeEndereco) };
		}

		public Mensagem EnderecoMunicipioInvalido(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__MunicipioId", objPaiNome, lstEnderecosNome, index), Texto = String.Format("Município de {0} é inválido.", nomeEndereco) };
		}

		public Mensagem EnderecoMunicipioOutroEstado(string objPaiNome, string lstEnderecosNome, int index, string nomeEndereco)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__MunicipioId", objPaiNome, lstEnderecosNome, index), Texto = String.Format("O estado do município de {0} selecionado é diferente do estado selecionado.", nomeEndereco) };
		}

		public Mensagem EnderecoZonaLocalizacaoObrigatoria(string objPaiNome, string lstEnderecosNome, int index)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = String.Format("{0}_{1}_{2}__ZonaLocalizacaoId", objPaiNome, lstEnderecosNome, index), Texto = "Zona de localização é obrigatória." };
		}
	}
}