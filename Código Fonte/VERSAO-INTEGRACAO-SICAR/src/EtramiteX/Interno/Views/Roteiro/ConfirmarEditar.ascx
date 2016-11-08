<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro.Roteiro>" %>

<h2 class="titTela"> Alterar Versão </h2>
<br />
<p>Ao editar este roteiro a sua versão será alterada de <%= Model.Versao %> para <%= Model.Versao + 1 %>. Deseja confirmar a edição?</p>