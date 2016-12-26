<%@ Import Namespace="Tecnomapas.Blocos.Entities.Configuracao.Interno" %>
<%@ Import  Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<MotivoTramitacaoVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Configurar Motivo de Tramitação</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script src="<%= Url.Content("~/Scripts/Tramitacao/motivo.js") %>" ></script>

	<script>
		Motivo.Mensagem = <%= Model.Mensagens %>;
		Motivo.settings.UrlSalvarMotivo = '<%= Url.Action("ConfigurarMotivoSalvar", "Tramitacao") %>';
		Motivo.settings.UrlAtivarMotivo = '<%= Url.Action("ConfigurarMotivoAtivar", "Tramitacao") %>';
		Motivo.settings.UrlDesativarMotivo = '<%= Url.Action("ConfigurarMotivoDesativar", "Tramitacao") %>';

		$(function () {
			Motivo.load($('#central'));
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<div id="central">
	<h1 class="titTela">Configurar Motivo de Tramitação</h1>
	<br />

	<fieldset class="block box">
			<legend>Configuração de Motivos</legend>

				<div class="block fixado">
					<div class="coluna98">
						<div class="coluna85">
							<label class="Motivo.Nome">Nome</label>
							<input type="text" value=""  maxlength="100" class="text txtMotivo setarFoco"/>
						</div>
							<div class="coluna10">
								<button type="button" style="width:35px" class="inlineBotao botaoAdicionarIcone btnAdicionarMotivo" title="Adicionar Motivo">Adicionar</button>
								<button type="button" style="width:30px" class="inlineBotao botaoSalvarIcone btnSalvarMotivo" title="Salvar Motivo">Salvar</button>
								<button type="button" style="width:30px" class="inlineBotao botaoCancelarIcone btnCancelarMotivo" title="Cancelar Motivo"
				>Cancelar</button>
							</div>
					</div>
				</div>


			<div class="block clear">
				<div class="dataGrid">
					<table class="tabItensMotivo dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
						<thead>
							<tr>
								<th>Nome</th>
								<th width="11%">Situação</th>
								<th width="11%">Ações</th>
							</tr>
						</thead>
						<tbody>

							<% foreach(Motivo motivo in Model.Motivos){	%>

									<tr>
										<td>
											<input type="hidden" class="hdnItemId" value="<%: motivo.Id %>" />
											<input type="hidden" class="hdnItemAtivo" value="<%: motivo.IsAtivo %>" />
											<span class="trItemMotivoNome" title=""><%: motivo.Nome %></span>
										</td>
										<td>
											<span class="trItemMotivoSituacao" title=""> <%:  motivo.IsAtivo ? "Ativo" : "Desativo" %></span>
										</td>
										<td>
											<input title="Editar" type="button" class="icone editar btnEditarMotivo" value="" />
										<%if( motivo.IsAtivo) { %>
													<input title="Desativar Motivo" type="button" class="icone cancelar btnDesativarMotivo" value="" />
										<% }else{ %>
													<input title="Ativar Motivo" type="button" class="icone recebido btnAtivarMotivo" value="" />
										<% } %>
										</td>
									</tr>

								<% } %>

						</tbody>
					</table>
				</div>
			</div>
				<table class="hide">
					<tbody>
								<tr class="trTemplate">
									<td>
										<input type="hidden" class="hdnItemId" value="0" />
										<input type="hidden" class="hdnItemAtivo" value="true" />
										<span class="trItemMotivoNome" ></span>
									</td>
									<td>
										<span class="trItemMotivoSituacao" ></span>
									</td>
									<td>
										<input title="Editar" type="button" class="icone editar btnEditarMotivo" />
										<input title="Desativar Motivo" type="button" class="icone cancelar btnDesativarMotivo"  />
										<input title="Ativar Motivo" type="button" class="icone recebido btnAtivarMotivo"  />
									</td>
								</tr>
							</tbody>
						</table>

		</fieldset>
</div>
</asp:Content>


