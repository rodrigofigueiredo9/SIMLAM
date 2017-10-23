<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao" %>
<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ConsideracaoFinalVM>" %>
<script src="<%= Url.Content("~/Scripts/arquivoFiscalizacao.js") %>"></script>
<script>

	FiscalizacaoConsideracaoFinal.settings.urls.salvar = '<%: Url.Action("SalvarConsideracaoFinal", "Fiscalizacao") %>';
	FiscalizacaoConsideracaoFinal.settings.urls.obter = '<%: Url.Action("ConsideracaoFinal", "Fiscalizacao") %>';
	FiscalizacaoConsideracaoFinal.settings.urls.obterSetores = '<%: Url.Action("ObterSetores", "Fiscalizacao") %>';
    FiscalizacaoConsideracaoFinal.settings.urls.obterEnderecoSetor = '<%: Url.Action("ObterEnderecoSetor", "Fiscalizacao") %>';
    FiscalizacaoConsideracaoFinal.settings.urls.obterCPF = '<%: Url.Action("ObterCPF", "Fiscalizacao") %>';
	FiscalizacaoConsideracaoFinal.settings.urls.enviarArquivo = '<%= Url.Action("Arquivo", "Arquivo") %>';
	FiscalizacaoConsideracaoFinal.settings.urls.obterAssinanteCargos = '<%= Url.Action("ObterAssinanteCargos", "Fiscalizacao") %>';
    FiscalizacaoConsideracaoFinal.settings.urls.obterAssinanteFuncionarios = '<%= Url.Action("ObterAssinanteFuncionarios", "Fiscalizacao") %>';
    FiscalizacaoConsideracaoFinal.settings.urls.associarTestemunha= '<%= Url.Action("PessoaModal", "Pessoa") %>';
    FiscalizacaoConsideracaoFinal.settings.urls.editarTestemunha = '<%= Url.Action("PessoaModalVisualizar", "Pessoa") %>';
	FiscalizacaoConsideracaoFinal.settings.mensagens = <%= Model.Mensagens %>;
	FiscalizacaoConsideracaoFinal.TiposArquivo = <%= Model.TiposArquivoValido %>;

</script>

<div class="divConsideracaoFinal">
	<%= Html.Hidden("hdnConsideracaoFinalId", Model.ConsideracaoFinal.Id, new { @class = "hdnConsideracaoFinalId" })%>
	<div class="block box">
		<div class="block">
			<div class="ultima">
				<label for="ConsideracaoFinal_Descrever">Descrever outras informações que julgar relevante para um maior detalhamento e esclarecimento do processo</label>
				<%= Html.TextArea("ConsideracaoFinal.Descrever", Model.ConsideracaoFinal.Descrever, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text textarea txtDescrever" }))%>
			</div>
		</div>
		<div class="block">
			<div class="coluna42">
				<label for="">Há necessidade de reparação do dano? *</label><br />

				<span style="border-style: solid; border-width: 1px; border-color: transparent;" class="text" id="rblOpinar">
					<label><%= Html.RadioButton("rblOpinar", 1, Model.ConsideracaoFinal.HaReparacao == 1, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblOpinar prepend2" }))%>Sim</label>
					<label><%= Html.RadioButton("rblOpinar", 0, Model.ConsideracaoFinal.HaReparacao == 0, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblOpinar" }))%>Não</label>
                    <label><%= Html.RadioButton("rblOpinar", -1, Model.ConsideracaoFinal.HaReparacao == -1, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblOpinar" }))%>Não se aplica</label>
				</span>
			</div>
		</div>
		<div class="block divReparacaoSim">
			<div class="ultima">
				<label for="ConsideracaoFinal_Reparacao">Opinar quanto à forma de reparação ou justificar caso esta não seja necessária *</label>
				<%= Html.TextArea("ConsideracaoFinal.Reparacao", Model.ConsideracaoFinal.Reparacao, ViewModelHelper.SetaDisabledReadOnly(Model.IsVisualizar, new { @class = "text media textarea txtOpinarReparacao", @maxlength = "2000" }))%>
			</div>
		</div>
		<div class="block divTermo<%= Model.ConsideracaoFinal.HaReparacao.HasValue && Model.ConsideracaoFinal.HaReparacao.Value == 1 ? "" : " hide"%>">
			<div class="block">
				<div class="coluna60">
					<label for="">Firmou termo de compromisso para reparação do dano de acordo com a forma sugerida? *</label><br />
					<span style="border-style: solid; border-width: 1px; border-color: transparent;" class="text" id="spanRblTermo">
						<label><%= Html.RadioButton("rblTermo", 1, Model.ConsideracaoFinal.HaTermoCompromisso.HasValue && Model.ConsideracaoFinal.HaTermoCompromisso.Value, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblTermo prepend2" }))%>Sim</label>
						<label><%= Html.RadioButton("rblTermo", 0, Model.ConsideracaoFinal.HaTermoCompromisso.HasValue && !Model.ConsideracaoFinal.HaTermoCompromisso.Value, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @class = "radio rblTermo" }))%>Não</label>
					</span>
				</div>
			</div>

			<div class="block divArquivo<%= Model.ConsideracaoFinal.HaTermoCompromisso.GetValueOrDefault() ? "" : " hide" %>">
				<div class="coluna40 inputFileDiv">
					<label>PDF do termo</label>
					<div class="block">
						<a href="<%= Url.Action("Baixar", "Arquivo", new { id = Model.ConsideracaoFinal.Arquivo.Id }) %>" class="<%= string.IsNullOrEmpty(Model.ConsideracaoFinal.Arquivo.Nome) ? "hide" : "" %> txtArquivoNome" target="_blank"><%= Html.Encode(Model.ConsideracaoFinal.Arquivo.Nome)%></a>
					</div>
					<input type="hidden" class="hdnArquivoJson" value="<%= Html.Encode(Model.ArquivoJSon) %>" />
					<span class="spanInputFile <%= string.IsNullOrEmpty(Model.ConsideracaoFinal.Arquivo.Nome) ? "" : "hide" %>">
						<input type="file" id="fileTermo" class="inputFileTermo text" style="display: block" name="file" <%=Model.IsVisualizar ? "disabled=\"disabled\"" : "" %>/>
					</span>
				</div>
				<% if (!Model.IsVisualizar) { %>
				<div style="margin-top:8px" class="coluna40 prepend1 spanBotoes">
					<button type="button" class="inlineBotao botaoAdicionar btnAddArq <%= string.IsNullOrEmpty(Model.ConsideracaoFinal.Arquivo.Nome) ? "" : "hide" %>" title="Enviar arquivo">Enviar</button>
					<button type="button" class="inlineBotao btnLimparArq <%= string.IsNullOrEmpty(Model.ConsideracaoFinal.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" >Limpar</button>
				</div>
				<% } %>
			</div>

			<div class="block divTermoJustificar<%= Model.ConsideracaoFinal.HaTermoCompromisso.HasValue && !Model.ConsideracaoFinal.HaTermoCompromisso.Value ? "" : " hide"%>">
				<div class="ultima">
					<label for="ConsideracaoFinal_TermoCompromissoJustificar">Justificar *</label>
					<%= Html.TextArea("ConsideracaoFinal.TermoCompromissoJustificar", Model.ConsideracaoFinal.TermoCompromissoJustificar, ViewModelHelper.SetaDisabled(Model.IsVisualizar, new { @maxlength = "250", @class = "txtTermoJustificar textarea media text" }))%>
				</div>
			</div>

		</div>
	</div>

	<fieldset class="block box fdsTestemunhas">
		<legend>Testemunha</legend>
	    <% Html.RenderPartial("ConsideracaoFinalTestemunha", Model.ConsideracaoFinalTestemunhaVM.First()); %>
   </fieldset>

   <% Html.RenderPartial("Assinantes", Model.AssinantesVM); %>

    <fieldset class="block box fsArquivosIUF">
		<legend>PDF do IUF</legend>
		<% Html.RenderPartial("~/Views/Arquivo/Arquivo.ascx", Model.ArquivoIUFVM); %>
	</fieldset>

	<fieldset class="block box fsArquivos">
		<legend>Relatório fotográfico</legend>
		<% Html.RenderPartial("~/Views/Arquivo/ArquivoFiscalizacao.ascx", Model.ArquivoVM); %>
	</fieldset>
</div>

