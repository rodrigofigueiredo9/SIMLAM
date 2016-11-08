namespace Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx
{
	public interface IConfiguracaoEvent
	{
		void AddExecutedAcao(ConfiguracaoDefault.Acao Acao);
		void AddLoadAcao(ConfiguracaoDefault.Acao Acao);
	}
}
