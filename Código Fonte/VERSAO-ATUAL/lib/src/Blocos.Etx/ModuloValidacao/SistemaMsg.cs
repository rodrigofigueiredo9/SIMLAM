using System;
using System.Collections.Generic;
using System.Linq;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static SistemaMsg _sistema = new SistemaMsg();
		public static SistemaMsg Sistema
		{
			get { return _sistema; }
			set { _sistema = value; }
		}
	}

	public class SistemaMsg
	{
		public Mensagem PermissaoNaoEncontrada(List<string> permissoes)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = String.Format("Permissão não encontrada no banco, \"{0}\".", Mensagem.Concatenar(permissoes)) };
		}

		public Mensagem SemPermissao(string nomePermissao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Usuário logado não possui a permissão \"{0}\".", nomePermissao) };
		}

		public Mensagem SemPermissoes(string nomePermissoes)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = String.Format("Usuário logado não possui nenhuma das seguintes permissões \"{0}\".", nomePermissoes) };
		}

		public Mensagem SemPermissao(List<string> nomePermissoes)
		{
			if (nomePermissoes.Count == 1)
			{
				return SemPermissao(nomePermissoes.First());
			}
			else
			{
				return SemPermissoes(Mensagem.Concatenar(nomePermissoes));
			}
		}

		public Mensagem ControleAcessoParametrosInsuficientes
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.ControleAcesso, Campo = "", Texto = "Erro ao Gerar Controle de acesso, favor colocar parametros corretamente na Action." }; }
		}

		public Mensagem CoordenadaForaMBR
		{
			get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "", Texto = "Coordenadas fora dos limites aceitáveis." }; }
		}
	}
}
