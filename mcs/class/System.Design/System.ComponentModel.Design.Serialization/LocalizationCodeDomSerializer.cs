using System;
using System.CodeDom;
using System.Collections;
using System.Resources;

namespace System.ComponentModel.Design.Serialization
{
	internal class LocalizationCodeDomSerializer : CodeDomSerializer
	{
		internal LocalizationCodeDomSerializer(CodeDomLocalizationModel model, object currentSerializer)
		{
			this._model = model;
			this._currentSerializer = (currentSerializer as CodeDomSerializer);
		}

		private bool EmitApplyMethod(IDesignerSerializationManager manager, object owner)
		{
			LocalizationCodeDomSerializer.ApplyMethodTable applyMethodTable = (LocalizationCodeDomSerializer.ApplyMethodTable)manager.Context[typeof(LocalizationCodeDomSerializer.ApplyMethodTable)];
			if (applyMethodTable == null)
			{
				applyMethodTable = new LocalizationCodeDomSerializer.ApplyMethodTable();
				manager.Context.Append(applyMethodTable);
			}
			if (!applyMethodTable.Contains(owner))
			{
				applyMethodTable.Add(owner);
				return true;
			}
			return false;
		}

		public override object Serialize(IDesignerSerializationManager manager, object value)
		{
			PropertyDescriptor propertyDescriptor = (PropertyDescriptor)manager.Context[typeof(PropertyDescriptor)];
			ExpressionContext expressionContext = (ExpressionContext)manager.Context[typeof(ExpressionContext)];
			bool flag = value == null || TypeDescriptor.GetReflectionType(value).IsSerializable;
			bool flag2 = !flag;
			bool flag3 = propertyDescriptor != null && propertyDescriptor.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);
			if (!flag2)
			{
				flag2 = (expressionContext != null && expressionContext.PresetValue == value);
			}
			if (this._model == CodeDomLocalizationModel.PropertyReflection && !flag3 && !flag2)
			{
				CodeStatementCollection codeStatementCollection = (CodeStatementCollection)manager.Context[typeof(CodeStatementCollection)];
				bool flag4 = false;
				if (propertyDescriptor != null)
				{
					ExtenderProvidedPropertyAttribute extenderProvidedPropertyAttribute = propertyDescriptor.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
					if (extenderProvidedPropertyAttribute != null && extenderProvidedPropertyAttribute.ExtenderProperty != null)
					{
						flag4 = true;
					}
				}
				if (!flag4 && expressionContext != null && codeStatementCollection != null)
				{
					string text = manager.GetName(expressionContext.Owner);
					CodeExpression codeExpression = base.SerializeToExpression(manager, expressionContext.Owner);
					if (text != null && codeExpression != null)
					{
						RootContext rootContext = manager.Context[typeof(RootContext)] as RootContext;
						if (rootContext != null && rootContext.Value == expressionContext.Owner)
						{
							text = "$this";
						}
						base.SerializeToResourceExpression(manager, value, false);
						if (this.EmitApplyMethod(manager, expressionContext.Owner))
						{
							ResourceManager value2 = manager.Context[typeof(ResourceManager)] as ResourceManager;
							CodeExpression expression = base.GetExpression(manager, value2);
							CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(expression, "ApplyResources");
							codeStatementCollection.Add(new CodeMethodInvokeExpression
							{
								Method = method,
								Parameters = 
								{
									codeExpression,
									new CodePrimitiveExpression(text)
								}
							});
						}
						return null;
					}
				}
			}
			if (flag2)
			{
				return this._currentSerializer.Serialize(manager, value);
			}
			return base.SerializeToResourceExpression(manager, value);
		}

		private CodeDomLocalizationModel _model;

		private CodeDomSerializer _currentSerializer;

		private class ApplyMethodTable
		{
			internal bool Contains(object value)
			{
				return this._table.ContainsKey(value);
			}

			internal void Add(object value)
			{
				this._table.Add(value, value);
			}

			private Hashtable _table = new Hashtable();
		}
	}
}
 
