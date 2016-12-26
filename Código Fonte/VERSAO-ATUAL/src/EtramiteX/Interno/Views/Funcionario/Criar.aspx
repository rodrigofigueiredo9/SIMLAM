<%@ Import Namespace="Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Interno.Master" Inherits="System.Web.Mvc.ViewPage<CriarVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Cadastrar Funcionário
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script>
	$(function () {
		$('[name="Cpf"]').focus();
	});
</script>

<div id="central">
	<% if (Model.Funcionario != null && Model.Funcionario.Id > 0)
	{%>
	<h1 class="titTela">Editar Funcionário</h1>
	<% }
	else
	{  %>
	<h1 class="titTela">Cadastrar Funcionário</h1>
	<% }%>

	<br />

	<% if (Model.CpfValido != null && !Model.CpfValido)
	   {%>
	   <div class="block box">
		<% using (Html.BeginForm("VerificarCpf", "Funcionario", FormMethod.Get))
		   { %>
			<div class="block">
				<div class="coluna20">
                    <div class="CpfFuncionarioContainer">
					    <label for="CpfFuncionario">CPF *</label>
					    <%= Html.TextBox("Cpf", null, new { @class = "text maskCpf", tabindex="1" }) %>
                    </div>
				</div>
				<div class="coluna15">
					<input type="submit" id="btnVerificarCpf" value="Verificar" class="inlineBotao" tabindex="2" />
				</div>
			</div>

		</div>
		<% } %>

	<% }
	   else
	   {%>

		   <% using (Html.BeginForm("Criar", "Funcionario"))
			  { %>

			  <div class="block box">

				<div class="block">
					<div class="coluna20">
                        <div class="CpfFuncionarioContainer">
						    <label for="CpfFuncionario">CPF *</label>
						    <%= Html.TextBox("Cpf", null, new { @class = "	text disabled", readOnly = true, tabindex = "3" })%>
					    </div>
                    </div>
					<div class="coluna15">
						<button name="button" type="button" onclick="document.location.href=$('#limparCpf').attr('href')" class="inlineBotao" tabindex="4">Limpar</button>
						<a id="limparCpf" href="<%= Html.AttributeEncode(Url.Action("Criar", "Funcionario")) %>" style="display:none;" tabindex="5"></a>
					</div>
				</div>

				<div class="block">
					<div class="coluna32">
						<label for="Nome">Nome *</label>
						<%= Html.TextBox("Nome", Model.Funcionario.Nome, new { maxlength = "80", @class = "text", tabindex = "6" })%>
						<%= Html.ValidationMessage("Nome")%>
					</div>
				</div>

				<div class="block">
						<div class="coluna32">
							<label for="login">E-mail</label>
							<%= Html.TextBox("Email", Model.Funcionario.Email, new { maxlength = "100", @class = "text", tabindex = "7" })%>
							<%= Html.ValidationMessage("email")%>
						</div>
					</div>


				<div class="block">

					<div class="selectDataListaCaixa">
						<label for="cargo">Função *</label>
						<%= Html.DropDownList("ddlCargos", Model.Cargos, new { @class = "selectLista", type = "text", tabindex = "8" })%>
					</div>

					<div class="botaoInserirListaCaixa">
						<button type="button" class="botaoInserirLista" id="btnAddCargo" title="Inserir Função" tabindex="9">Inserir cargo</button>
					</div>

					<table style="display:none;">
						<tr class="linhaCargo">
							<td width="90%" class="celCargoTexto"></td>
							<td width="10%">
								<input type="hidden" name="ListaCargos" class="hdnCargoId" disabled="true" />
								<input type="button" class="icone excluir" onclick="Funcionario.onExcluirLinha(this);" title="Excluir" tabindex="10" />
							</td>
						</tr>
					</table>

					<div class="caixaScroll coluna100">
						<table id="tableCargo" class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
						<% for (int i = 0; i < Model.Funcionario.Cargos.Count; i++)
							{ %>
								<tr>
									<td width="90%"><%= Html.Label(Model.Funcionario.Cargos[i].Nome)%></td>
									<td width="10%">
										<input type="hidden" name="ListaCargos" class="hdnCargoId" value="<%= Html.Encode(Model.Funcionario.Cargos[i].Id)%>" />
										<input type="button" class="icone excluir" onclick="Funcionario.onExcluirLinha(this);" title="Excluir" tabindex="10" />
									</td>
								</tr>
						<% } %>
						</table>
					</div>
				</div>

				<div>

					<div class="selectDataListaCaixa">
						<label for="setor">Setor *</label>
						<%= Html.DropDownList("ddlSetores", Model.Setores, new { @class = "selectLista", type = "text", tabindex = "20" })%>
					</div>

					<div class="botaoInserirListaCaixa">
						<button type="button" class="botaoInserirLista" id="btnAddSetor" title="Inserir Setor" tabindex = "21" >Inserir setor</button>
					</div>

					<table style="display:none;">
						<tr class="linhaSetor par">
							<td width="60%" class="celSetorTexto"></td>
							<td width="30%" class="celSetorResp handclick">
								<div class="hide"><input type="checkbox" class="ckbSetor" tabindex="22" name="ListaSetores[].EhResponsavel" value="true" /><label> Responsável</label></div>
							</td>
							<td width="10%">
								<input type="hidden" name="ListaSetores.index" class="hdnSetorIndex" disabled="true" />
								<input type="hidden" name="ListaSetores[].Id" class="hdnSetorId" disabled="true" />
								<input type="button" class="icone excluir" onclick="Funcionario.onExcluirLinha(this);" title="Excluir" tabindex = "23" />
							</td>
						</tr>
					</table>
					<div class="caixaScroll coluna100">
						<table id="tableSetor" class="dataGridTable" width="100%" border="0" cellspacing="0" cellpadding="0">
						<% for (int i = 0; i < Model.Funcionario.Setores.Count; i++)
							{ %>
								<tr>
									<td width="60%"><%= Html.Label(Model.Funcionario.Setores[i].Nome)%></td>
									<td width="30%" class="celSetorResp handclick">
									<% if (Model.Funcionario.Setores[i].EhResponsavel || (Model.Funcionario.Setores[i].Responsavel ?? 0) == 0)
									{ %>
									<input type="checkbox" class="ckbSetor" tabindex="22" name="<%= "ListaSetores["+Model.Funcionario.Setores[i].Id +"].EhResponsavel" %>" value="true" <%= (Model.Funcionario.Setores[i].EhResponsavel)?"checked=\"checked\"":"" %> />  <label> Responsável</label></td>
									<% } %>
									<td width="10%">
										<input type="hidden" name="ListaSetores.index" class="hdnSetorIndex" value="<%= Html.Encode(Model.Funcionario.Setores[i].Id)%>" />
										<input type="hidden" name="<%= "ListaSetores["+Html.Encode(Model.Funcionario.Setores[i].Id)+"].Id"%>" class="hdnSetorId" value="<%= Html.Encode(Model.Funcionario.Setores[i].Id)%>" />
										<input type="button" class="icone excluir" onclick="Funcionario.onExcluirLinha(this);" title="Excluir" tabindex="23" />
									</td>
								</tr>
						<% } %>
						</table>
					</div>

				</div>

		</div>

		<div class="block box">
			<div class="coluna48 border">

				<div class="caixaAbas">
				<div id="abasNav">

					<ul class="ui-tabs-nav">
						<li class="ui-tabs-nav-item ui-tabs-selected" id="nav-fragment-1">
							<a href="#fragment-1" class="primeira"> Papéis </a>
						</li>
					</ul>

					<div id="fragment-1" class="ui-tabs-panel" >
						<div class="block boxEscuro">
							<div id="divPapeis" class="caixaScroll">
								<div class="handclick">
									<table id="tablePapeis" class="dataGridTable" border="0" cellpadding="0" cellspacing="0" width="100%">
									<tbody>
									<% for (int i = 0; i < Model.Papeis.Count; i++)
									   {%>
										<%= Html.HiddenFor(x => x.Papeis[i].Papel.Id)%>

											<tr class="<%= ((Model.Papeis[i].IsAtivo)?"linhaSelecionada":string.Empty) %>" >
												<td class="celulaSeletorLinha" width="8%"><%= Html.CheckBoxFor(x => x.Papeis[i].IsAtivo, new { @type = "checkbox", tabindex="24"})%></td>
												<td class="celulaSeletorLinha" width="92%"><%= Html.Encode(Model.Papeis[i].Papel.Nome)%> </td>
											</tr>

									<% } %>
									</tbody>
									</table>
								</div>
							</div>
						</div>
					</div>

				</div>
				</div>

			</div>

			<div class="coluna48 ultima">
				<div class="paddingT10">
					<label for="PermissoesConcedidas">Permissões concedidas</label>
				</div>
				<div>
					<%= Html.TextArea("TextoPermissoes", Model.TextoPermissoes, new { readOnly = "true", @class = "textarea caixaPapeis", tabindex = "800" })%>
				</div>
			</div>

		</div>


            <div class="block box">
                    <div class="block">
			        <div class="coluna80 inputFileDiv">
				        <label for="ArquivoTexto">Assinatura Digitalizada</label>
                        <% if(!string.IsNullOrEmpty(Model.Funcionario.Arquivo.Nome)) { %>
                                <%= Html.TextBox("txtArquivoNome", Model.Funcionario.Arquivo.Nome, new { readOnly = "true", @class = "text txtArquivoNome disabled", @disabled = "disabled" })%>
                        <% } %>
                        <% else { %>
				                <%= Html.TextBox("txtArquivoNome", null, new { readOnly = "true", @class = "text txtArquivoNome hide disabled", @disabled = "disabled" })%>
                        <% } %>


                        <span class="spanInputFile <%= string.IsNullOrEmpty(Model.Funcionario.Arquivo.Nome) ? "" : "hide" %>">
				        <input type="file" id="file" class="inputFile" style="display: block; width: 100%" name="file" /></span>
				        <input type="hidden" class="hdnAnexoArquivoJson" name="hdnAnexoArquivoJson" value="<%= Model.ObterJSon(Model.Funcionario.Arquivo) %>" />

                        <input type="hidden" class="hdnArquivoContentType" id="ArquivoContentType" name="ArquivoContentType" value="<%= Model.Funcionario.Arquivo.ContentType %>" />
                        <input type="hidden" class="hdnArquivoExtensao" id="ArquivoExtensao" name="ArquivoExtensao" value="<%= Model.Funcionario.Arquivo.Extensao %>" />
                        <input type="hidden" class="hdnArquivoId" id="ArquivoId" name="ArquivoId" value="<%= Model.Funcionario.Arquivo.Id %>" />
                        <input type="hidden" class="hdnArquivoNome" id="ArquivoNome" name="ArquivoNome" value="<%= Model.Funcionario.Arquivo.Nome %>" />
                        <input type="hidden" class="hdnArquivoTemporarioNome" id="ArquivoTemporarioNome" name="ArquivoTemporarioNome" value="<%= Model.Funcionario.Arquivo.TemporarioNome %>" />
			        </div>

			        <div class="block ultima spanBotoes">
				        <button type="button" class="inlineBotao btnArq <%= string.IsNullOrEmpty(Model.Funcionario.Arquivo.Nome) ? "" : "hide" %>" title="Enviar anexo" onclick="Funcionario.onEnviarAnexoArquivoClick('<%= Url.Action("arquivo", "arquivo") %>');">Enviar</button>
				        <button type="button" class="inlineBotao btnArqLimpar <%= string.IsNullOrEmpty(Model.Funcionario.Arquivo.Nome) ? "hide" : "" %>" title="Limpar arquivo" onclick="Funcionario.onLimparArquivo()" ><span>Limpar</span></button>
			        </div>
		        </div>
        </div>



		<div class="block box">

					<div class="block">
						<div class="coluna20">
						<p>
							<label for="login">Login *</label>
							<%= Html.TextBox("Login", Model.Funcionario.Usuario.Login, new { maxlength = "30", @class = "text", tabindex = "32" })%>
							<%= Html.ValidationMessage("login")%>
						</p>
							<%= Html.Hidden("forcaSenha") %>
							<label for="senha">Senha *</label>
							<%= Html.Password("Senha", null, new { @class = "text", type = "password", tabindex = "33" })%>
							<%= Html.ValidationMessage("senha")%>
						<p>
							<label for="ConfirmarSenha">Confirmar senha *</label>
							<%= Html.Password("ConfirmarSenha", null, new { @class = "text", type = "password", tabindex = "34" })%>
							<%= Html.ValidationMessage("ConfirmarSenha")%>
						</p>

						</div>

						<div class="coluna50 prepend2">

						</div>
					</div>


		</div>

		<div class="block box">
			<input id="salvar" type="submit" value="Salvar" class="floatLeft" tabindex = "35" />
			<span class="cancelarCaixa">ou <a class="linkCancelar" tabindex = "36" href="<%= Url.Action("") %>" >Cancelar</a></span>
		</div>

		<% }  %>

	<script>
		$("#login").focus();
		var urlPermissao = '<%= Url.Content("~/Funcionario/TextoPermissoes") %>';
	</script>

	<% } %>

</div>

<script src="<%= Url.Content("~/Scripts/Funcionario/funcionario.js") %>" ></script>
<script>
	Funcionario.urlVerificarResponsavelSetor = '<%= Url.Action("VerificarResponsavelSetor","Funcionario") %>';
	Funcionario.settings.mensagens = <%=Model.Mensagens%>;
</script>

</asp:Content>

