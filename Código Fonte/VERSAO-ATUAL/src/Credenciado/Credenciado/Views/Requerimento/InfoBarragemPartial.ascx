<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Requerimento>" %>

<h1 class="titTela">Informações da Barragem</h1>
<br />

<div class="block box">
	<div class="block">
		<label>Esta declaração de dispensa tem como objetivo a regularização de barragem destinada ao abastecimento público? *</label><br />
		<label>
				<%=Html.RadioButton("Requerimento.AbastecimentoPublico", 1, false, ViewModelHelper.SetaDisabled(false, new { @class="rbInfoBarragem rbAbastecimentoPublico"}))%>
				Sim
		</label>
		<label>
				<%=Html.RadioButton("Requerimento.AbastecimentoPublico", 0, false, ViewModelHelper.SetaDisabled(false, new { @class="rbInfoBarragem rbAbastecimentoPublico"}))%>
				Não
		</label>
	</div>
	<div class="block">
		<label>A barragem está localizada em Unidade de Conservação ou em Zona de Amortecimento? *</label><br />
		<label>
				<%=Html.RadioButton("Requerimento.UnidadeConservacao", 1, false, ViewModelHelper.SetaDisabled(false, new { @class="rbInfoBarragem rbUnidadeConservacao"}))%>
				Sim
		</label>
		<label>
				<%=Html.RadioButton("Requerimento.UnidadeConservacao", 0, false, ViewModelHelper.SetaDisabled(false, new { @class="rbInfoBarragem rbUnidadeConservacao"}))%>
				Não
		</label>
	</div>
	<div class="block">
		<label>Há (houve) necessidade de supressão de vegetação em estágio médio de regeneração para implantação da barragem? *</label><br />
		<label>
				<%=Html.RadioButton("Requerimento.SupressaoVegetacao", 1, false, ViewModelHelper.SetaDisabled(false, new { @class="rbInfoBarragem rbSupressaoVegetacao"}))%>
				Sim
		</label>
		<label>
				<%=Html.RadioButton("Requerimento.SupressaoVegetacao", 0, false, ViewModelHelper.SetaDisabled(false, new { @class="rbInfoBarragem rbSupressaoVegetacao"}))%>
				Não
		</label>
	</div>
	<div class="block">
		<label>Haverá necessidade de realocação de núcleos populacionais ou rodovias? *</label><br />
		<label>
				<%=Html.RadioButton("Requerimento.Realocacao", 1, false, ViewModelHelper.SetaDisabled(false, new { @class="rbInfoBarragem rbRealocacao"}))%>
				Sim
		</label>
		<label>
				<%=Html.RadioButton("Requerimento.Realocacao", 0, false, ViewModelHelper.SetaDisabled(false, new { @class="rbInfoBarragem rbRealocacao"}))%>
				Não
		</label>
	</div>
	<div class="block divBarragensContiguas hide">
		<label>Esta declaração de dispensa está sendo elaborada para barragens contíguas em um mesmo imóvel? *</label><br />
		<label>
				<%=Html.RadioButton("Requerimento.BarragensContiguas", 1, false, ViewModelHelper.SetaDisabled(false, new { @class="rbBarragensContiguas"}))%>
				Sim
		</label>
		<label>
				<%=Html.RadioButton("Requerimento.BarragensContiguas", 0, false, ViewModelHelper.SetaDisabled(false, new { @class="rbBarragensContiguas"}))%>
				Não
		</label>
	</div>
</div>