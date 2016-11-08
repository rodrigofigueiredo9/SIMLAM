using System;
using System.Globalization;
using System.Web.Mvc;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Business
{
	public class DecimalModelBinder : DefaultModelBinder
	{
		//http://msdn.microsoft.com/en-us/library/gg416514(v=vs.108).aspx 
		//Procurar por 'Decimal'

		#region Implementation of IModelBinder
		public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

			if (valueProviderResult.AttemptedValue.Equals("N.aN") ||
				valueProviderResult.AttemptedValue.Equals("NaN") ||
				valueProviderResult.AttemptedValue.Equals("Infini.ty") ||
				valueProviderResult.AttemptedValue.Equals("Infinity") ||
				valueProviderResult.AttemptedValue.Equals("null") ||
				valueProviderResult.AttemptedValue.Equals("undefined") ||
				String.IsNullOrEmpty(valueProviderResult.AttemptedValue))
			{
				return 0m;
			}

			//Notacao cientifica
			if (valueProviderResult.AttemptedValue.IndexOf("e", StringComparison.InvariantCultureIgnoreCase) > -1)
			{
				return decimal.Parse(valueProviderResult.AttemptedValue, NumberStyles.Any, CultureInfo.InvariantCulture);
			}

			//O parse do decimal nao funciona muito bem. Se converter pra double perde precisao.
			return valueProviderResult == null ? base.BindModel(controllerContext, bindingContext) : Convert.ToDecimal(valueProviderResult.AttemptedValue);
		}
		#endregion
	}
}