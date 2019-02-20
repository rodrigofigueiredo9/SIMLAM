<%@ Import Namespace="Tecnomapas.EtramiteX.Configuracao" %>
<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<BarragemDispensaLicencaVM>" %>

<script src="<%= Url.Content("~/Scripts/Areas/GeoProcessamento/coordenada.js") %>"></script>
<script>
	BarragemDispensaLicenca.settings.mensagens = <%= Model.Mensagens %>;
	BarragemDispensaLicenca.settings.idsTela = <%= Model.IdsTela %>;
</script>

<input type="hidden" class="hdnEmpreendimentoId" value="<%: Model.Caracterizacao.EmpreendimentoID %>" />
<input type="hidden" class="hdnCaracterizacaoId" value="<%: Model.Caracterizacao.Id %>" />

<fieldset class="block box">
	<legend class="titFiltros">Atividade Dispensada</legend>

	<div class="block boxBranca">
		<div class="coluna80">
			<label for="Atividade">Atividade *</label>
			<%= Html.DropDownList("Atividade", Model.Atividades, ViewModelHelper.SetaDisabled(true, new { @class = "text ddlAtividade" }))%>
		</div>
		
	</div>

	<div class="block boxBranca">
		<div>
			<label>Esta declaração de dispensa tem como objetivo a regularização de barragem destinada ao abastecimento público?</label><br />
			<%= Html.RadioButton("PerguntaAtividade1", 1, false, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbPerguntaAtiv1" })) %> Sim
			<%= Html.RadioButton("PerguntaAtividade1", 0, true, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbPerguntaAtiv1" })) %> Não
		</div>
		<div>
			<label>A barragem está localizada em Unidade de Conservação ou em Zona de Amortecimento?</label><br />
			<%= Html.RadioButton("PerguntaAtividade2", 1, false, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbPerguntaAtiv2" })) %> Sim
			<%= Html.RadioButton("PerguntaAtividade2", 0, true, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbPerguntaAtiv2" })) %> Não
		</div>
		<div>
			<label>Há (houve) necessidade de supressão de vegetação em estágio médio de regeneração para implantação da barragem?</label><br />
			<%= Html.RadioButton("PerguntaAtividade3", 1, false, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbPerguntaAtiv3" })) %> Sim
			<%= Html.RadioButton("PerguntaAtividade3", 0, true, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbPerguntaAtiv3" })) %> Não
		</div>
		<div>
			<label>Haverá necessidade de realocação de núcleos populacionais ou rodovias?</label><br />
			<%= Html.RadioButton("PerguntaAtividade4", 1, false, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbPerguntaAtiv4" })) %> Sim
			<%= Html.RadioButton("PerguntaAtividade4", 0, true, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbPerguntaAtiv4" })) %> Não
		</div>
		<div>
			<label>Esta declaração de dispensa está sendo elaborada para barragens contíguas num mesmo imóvel?</label><br />
			<%= Html.RadioButton("PerguntaAtividade", ConfiguracaoSistema.SIM, Model.Caracterizacao.barragemContiguaMesmoNivel, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbBarragemContiguaMesmoNivel" })) %> Sim
			<%= Html.RadioButton("PerguntaAtividade", ConfiguracaoSistema.NAO, !Model.Caracterizacao.barragemContiguaMesmoNivel, ViewModelHelper.SetaDisabled(true, new { @class = "radio rbBarragemContiguaMesmoNivel" })) %> Não
		</div>
	</div>

	<fieldset class="block boxBranca">
		<legend>Dados da barragem</legend>
		
		<div class="divDescricaoBarragem">
			<%Html.RenderPartial("DescricaoGeralBarragem", Model);%>
		</div>
		
		<div class="divBarragemContruida <%=(Model.Caracterizacao.Fase == (int)eFase.Construida) ? "" : "hide"%>">
			<%Html.RenderPartial("DescContruida", Model);%>
		</div>

		<div class="divBarragemAContruir <%=(Model.Caracterizacao.Fase == (int)eFase.AConstruir) ? "" : "hide"%>">
			<%Html.RenderPartial("DescAContruir", Model);%>
		</div>

	</fieldset>
	
	<div class="divResponsabilidadeTecnica">
		<%Html.RenderPartial("ResponsabilidadeTecnica", Model);%>
	</div>
</fieldset>