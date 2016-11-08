<%@ Import Namespace="Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<InfracaoVM>" %>

<%= Html.Hidden("hdnConfiguracaoId", Model.Infracao.ConfiguracaoId, new { @class = "hdnConfiguracaoId" })%> 
<%= Html.Hidden("hdnConfiguracaoTid", Model.Infracao.ConfiguracaoTid, new { @class = "hdnConfiguracaoTid" })%> 

<div id="divCampos" class="divCampos" >
<% foreach (InfracaoCampo campo in Model.Campos) { %>

	<div class="block divCampo">
		<%= Html.Hidden("hdnCampoId", campo.CampoId, new { @class = "hdnCampoId" })%> 
		<%= Html.Hidden("hdnCampoInfracaoId", campo.Id, new { @class = "hdnCampoInfracaoId" })%> 
		<div class="coluna76">			
			<%= Html.Label(campo.Identificacao)%> <% if (!string.IsNullOrWhiteSpace(campo.UnidadeTexto)) { %>  - <%= Html.Label(campo.UnidadeTexto)%> <% } %> *<br />
			<%= Html.TextBox("campo.Texto", campo.Texto, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtTexto " + campo.Mascara, @maxlength = campo.Tamanho }))%>
		</div>
	</div>

<% } %>
</div>

<div id="divQuestionarios" class="divQuestionarios" >
<% foreach (InfracaoPergunta questionario in Model.Perguntas){ %>

	<%Boolean respostaEspecificar = false;%>

	<div class="divQuestionario" >	
		<%= Html.Hidden("hdnPerguntaId", questionario.PerguntaId, new { @class = "hdnPerguntaId" })%>
		<%= Html.Hidden("hdnPerguntaTid", questionario.PerguntaTid, new { @class = "hdnPerguntaTid" })%>
		<%= Html.Hidden("hdnQuestionarioId", questionario.Id, new { @class = "hdnQuestionarioId" })%> 
		<label><%= questionario.Identificacao%> *</label><br />
		<div class="block">
			<div class="coluna76">
			<% foreach (InfracaoResposta resposta in questionario.Respostas){ %>
				<label>
					<%= Html.RadioButton("pergunta.Resposta" + questionario.PerguntaId, resposta.Id, resposta.Id == questionario.RespostaId, ViewModelHelper.SetaDisabled(Model.IsVisualizar ,new { @perguntaId = questionario.PerguntaId, @class = "radio rdoResposta " + (resposta.IsEspecificar ? "rdoIsEspecificar" : "rdoIsNotEspecificar") }))%>
					<%= Html.Label(resposta.Identificacao)%>
					<%= Html.Hidden("hdnRespostaTid" + resposta.Id, resposta.Tid, new { @class = "hdnRespostaTid" + resposta.Id })%>
				</label>

				<%if (resposta.Id == questionario.RespostaId) { respostaEspecificar = resposta.IsEspecificar; }%>

			<% } %>
			</div>
		</div>
		<div class="block <%= "divEspecificacao" + questionario.PerguntaId %> <%= (respostaEspecificar) ? "" : " hide" %>  ">
			<div class="coluna76">
				<label>Especificar *</label><br />
				<%= Html.Hidden("hdnRespostaEspecificar", (respostaEspecificar) ? 1 : 0, new { @class = "hdnRespostaEspecificar hdnRespostaEspecificar" + questionario.PerguntaId })%> 
				<%= Html.TextBox("pergunta.Especificar",questionario.Especificacao, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "text txtEspecificar", @maxlength = "100" }))%>
			</div>
		</div>
	</div>
<% } %>
</div>