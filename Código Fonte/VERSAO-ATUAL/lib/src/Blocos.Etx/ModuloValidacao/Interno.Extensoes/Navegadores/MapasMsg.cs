namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static MapaCoordenadasMsg _mapasMsg = new MapaCoordenadasMsg();
		public static MapaCoordenadasMsg Mapas
		{
			get { return _mapasMsg; }
			set { _mapasMsg = value; }
		}
	}

	public class MapaCoordenadasMsg
	{
		public Mensagem MunicipioSemRetorno { get { return new Mensagem() { Tipo = eTipoMensagem.Erro, Campo = "", Texto = "Município não encontrado pelo webservice do Geobases." }; } }
	}
}
