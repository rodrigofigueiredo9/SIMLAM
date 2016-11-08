<%@ Import Namespace="Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital" %>
<%@ Import Namespace="Tecnomapas.EtramiteX.Credenciado.ViewModels.VMProjetoDigital" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Credenciado.Master" Inherits="System.Web.Mvc.ViewPage<ProjetoDigitalVM>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">Projeto Digital</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="JsHeadContent" runat="server">
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/ProjetoDigital/projetoDigital.js") %>"></script>
	<script type="text/javascript" src="<%= Url.Content("~/Scripts/containerAcoes.js") %>"></script>

	<script type="text/javascript">
		$(function () {
			ProjetoDigital.load($('#central'), {
				projetoDigitalId: '<%: Model.ProjetoDigital.Id %>',
				projetoDigitalEtapa: '<%: Model.ProjetoDigital.Etapa %>',
				projetoDigitalSituacao: '<%: Model.ProjetoDigital.Situacao %>',
				situacoesEditaveis: ['<%: (int)eProjetoDigitalSituacao.EmElaboracao %>', '<%: (int)eProjetoDigitalSituacao.AguardandoCorrecao %>'],
				modoVisualizar: eval('<%: Model.ModoVisualizar.ToString().ToLower() %>'),
				desativarPasso4: eval('<%: Model.DesativarPasso4.ToString().ToLower() %>'),
				urls: {
					requerimento: '<%: Model.UrlRequerimento %>',
					caracterizacao: '<%: Model.UrlCaracterizacao %>',
					enviar: '<%: Model.UrlEnviar %>',
					imprimirDocumentos: '<%: Model.UrlImprimirDocumentos %>',
					editarCaracterizacaoValidar: '<%= Url.Action("EditarCaracterizacaoValidarConfirm", "ProjetoDigital") %>',
					alterarDados: '<%= Url.Action("AlterarDados", "ProjetoDigital")%>'
				}
			});

			<% if (!String.IsNullOrEmpty(Request.Params["acaoId"])){%>
				ContainerAcoes.load($(".containerAcoes"), {
					botoes: new Array(
						{ label: 'Listar', url: '<%= Url.Action("Index", "ProjetoDigital") %>' },
						{ label: 'Cadastrar Novo', url: '<%= Url.Action("Operar", "ProjetoDigital") %>' }
						<% if (Model.PossuiAtividadeCAR) { %>, { label: 'Cadastrar Solicitação CAR', url: '<%= Url.Action("Criar", "CARSolicitacao", new { id = Request.Params["acaoId"].ToString() }) %>' } <% } %>
					)
				});
			<% } %>
		});
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
	<div id="central">
		<h1 class="titTela">Projeto Digital</h1>
		<br />

		<div class="paddingALL">
			<div class="block margemBottom margemTop">
				<% if (Model.ProjetoDigital.Situacao != (int)eProjetoDigitalSituacao.EmElaboracao && Model.ProjetoDigital.Situacao != (int)eProjetoDigitalSituacao.AguardandoCorrecao) { %>
					<div class="coluna22 passo1">
						<a class="linkProjDigital1 feito" title="Requerimento">Requerimento</a>
						<a href="<%= Model.UrlRequerimentoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Requerimento">Vizualizar</a>
					</div>
					<div class="coluna22 passo2">
						<a class="linkProjDigital2 feito" title="Caracterizações">Caracterizações</a>
						<a href="<%= Model.UrlCaracterizacaoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Caracterizações">Vizualizar</a>
					</div>
					<div class="coluna22 passo3">
						<a class="linkProjDigital3 feito" title="Enviar Projeto Digital">Enviar Projeto Digital</a>
						<a href="<%= Model.UrlEnviarVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Envio de Projeto Digital">Visualizar Envio de Projeto Digital</a>
					</div>
					<div class="coluna22 passo4 ultima">
						<a class="linkProjDigital4 feito" title="Imprimir documentos">Imprimir Documentos</a>
					</div>
				<% } else { %>
				<% switch(Model.ProjetoDigital.Etapa) { %>
					<% case 0: %>
					<div class="coluna22 passo1">
						<a class="linkProjDigital1 ativo" title="Requerimento">Requerimento</a>
					</div>
					<div class="coluna24 passo2">
						<span class="linkProjDigital2" title="Caracterizações">Caracterizações</span>
					</div>
					<div class="coluna18 passo3 ultima">
						<span class="linkProjDigital3" title="Enviar Projeto Digital">Enviar Projeto Digital</span>
					</div>
					<div class="coluna22 passo4 ultima">
						<a class="linkProjDigital4" title="Imprimir documentos">Imprimir Documentos</a>
					</div>
					<% break; %>

					<% case 1: %>
					<div class="coluna22 passo1">
						<a class="linkProjDigital1 ativo" title="Requerimento">Requerimento</a>
						<a href="<%= Model.UrlRequerimentoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Requerimento">Vizualizar</a>
					</div>
					<div class="coluna24 passo2">
						<span class="linkProjDigital2" title="Caracterizações">Caracterizações</span>
					</div>
					<div class="coluna18 passo3">
						<span class="linkProjDigital3" title="Enviar Projeto Digital">Enviar Projeto Digital</span>
					</div>
					<div class="coluna22 passo4 ultima">
						<a class="linkProjDigital4" title="Imprimir documentos">Imprimir Documentos</a>
					</div>
					<% break; %>

					<% case 2: %>
					<div class="coluna22 passo1">
						<a class="linkProjDigital1 feito" title="Requerimento">Requerimento</a>
						<a href="<%= Model.UrlRequerimentoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Requerimento">Vizualizar</a>
					</div>
					<div class="coluna24 passo2">
						<a class="linkProjDigital2 ativo" title="Caracterizações">Caracterizações</a>
						<a href="<%= Model.UrlCaracterizacaoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Caracterizações">Vizualizar</a>
					</div>
					<div class="coluna18 passo3">
						<span class="linkProjDigital3" title="Enviar Projeto Digital">Enviar Projeto Digital</span>
					</div>
					<div class="coluna22 passo4 ultima">
						<a class="linkProjDigital4" title="Imprimir documentos">Imprimir Documentos</a>
					</div>
					<% break; %>

					<% case 3: %>
					<div class="coluna22 passo1">
						<a class="linkProjDigital1 feito" title="Requerimento">Requerimento</a>
						<a href="<%= Model.UrlRequerimentoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Requerimento">Vizualizar</a>
					</div>
					<div class="coluna24 passo2">
						<a class="linkProjDigital2 feito" title="Caracterizações">Caracterizações</a>
						<a href="<%= Model.UrlCaracterizacaoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Caracterizações">Vizualizar</a>
					</div>
					<div class="coluna18 passo3">
						<a class="linkProjDigital3 <%= (Model.ProjetoDigital.Situacao == (int)eProjetoDigitalSituacao.AguardandoCorrecao ? "feito" : "ativo") %>" title="Enviar Projeto Digital">Enviar Projeto Digital</a>
						<a href="<%= Model.UrlEnviarVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Envio de Projeto Digital">Visualizar Envio de Projeto Digital</a>
					</div>
					<div class="coluna22 passo4 ultima">
						<a class="linkProjDigital4 <%= (Model.ProjetoDigital.Situacao == (int)eProjetoDigitalSituacao.AguardandoCorrecao ? "feito" : "ativo") %>" title="Imprimir documentos">Imprimir Documentos</a>
					</div>
					<% break; %>

					<% case 4: %>
					<div class="coluna22 passo1">
						<a class="linkProjDigital1 feito" title="Requerimento">Requerimento</a>
						<a href="<%= Model.UrlRequerimentoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Requerimento">Vizualizar</a>
					</div>
					<div class="coluna24 passo2">
						<a class="linkProjDigital2 feito" title="Caracterizações">Caracterizações</a>
						<a href="<%= Model.UrlCaracterizacaoVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Caracterizações">Vizualizar</a>
					</div>
					<div class="coluna18 passo3">
						<a class="linkProjDigital3 feito" title="Enviar Projeto Digital">Enviar Projeto Digital</a>
						<a href="<%= Model.UrlEnviarVisualizar %>" class="icone visualizar visualizarProjDigital" title="Visualizar Envio de Projeto Digital">Visualizar Envio de Projeto Digital</a>
					</div>
					<div class="coluna22 passo4 ultima">
						<a class="linkProjDigital4 feito" title="Imprimir documentos">Imprimir Documentos</a>
					</div>
					<% break; %>
				<% } %>
				<% } %>
			</div>
		</div>

		<div class="block box">
			<span class="cancelarCaixa"><a class="linkCancelar" title="Cancelar" href="<%= Url.Action("") %>">Cancelar</a></span>
		</div>
	</div>
</asp:Content>