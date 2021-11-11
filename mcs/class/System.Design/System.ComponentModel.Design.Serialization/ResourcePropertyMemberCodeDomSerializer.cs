using System;
using System.CodeDom;
using System.Globalization;

namespace System.ComponentModel.Design.Serialization
{
	internal class ResourcePropertyMemberCodeDomSerializer : MemberCodeDomSerializer
	{
		internal ResourcePropertyMemberCodeDomSerializer(MemberCodeDomSerializer serializer, CodeDomLocalizationProvider.LanguageExtenders extender, CodeDomLocalizationModel model)
		{
			this._serializer = serializer;
			this._extender = extender;
			this._model = model;
		}

		public override void Serialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
		{
			manager.Context.Push(this._model);
			try
			{
				this._serializer.Serialize(manager, value, descriptor, statements);
			}
			finally
			{
				manager.Context.Pop();
			}
		}

		private CultureInfo GetLocalizationLanguage(IDesignerSerializationManager manager)
		{
			if (this.localizationLanguage == null)
			{
				RootContext rootContext = manager.Context[typeof(RootContext)] as RootContext;
				if (rootContext != null)
				{
					object value = rootContext.Value;
					PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(value)["LoadLanguage"];
					if (propertyDescriptor != null && propertyDescriptor.PropertyType == typeof(CultureInfo))
					{
						this.localizationLanguage = (CultureInfo)propertyDescriptor.GetValue(value);
					}
				}
			}
			return this.localizationLanguage;
		}

		private void OnSerializationComplete(object sender, EventArgs e)
		{
			this.localizationLanguage = null;
			IDesignerSerializationManager designerSerializationManager = sender as IDesignerSerializationManager;
			if (designerSerializationManager != null)
			{
				designerSerializationManager.SerializationComplete -= this.OnSerializationComplete;
			}
		}

		public override bool ShouldSerialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor)
		{
			bool flag = this._serializer.ShouldSerialize(manager, value, descriptor);
			if (!flag && !descriptor.Attributes.Contains(DesignOnlyAttribute.Yes))
			{
				switch (this._model)
				{
				case CodeDomLocalizationModel.PropertyAssignment:
				{
					InheritanceAttribute inheritanceAttribute = (InheritanceAttribute)manager.Context[typeof(InheritanceAttribute)];
					if (inheritanceAttribute == null)
					{
						inheritanceAttribute = (InheritanceAttribute)TypeDescriptor.GetAttributes(value)[typeof(InheritanceAttribute)];
						if (inheritanceAttribute == null)
						{
							inheritanceAttribute = InheritanceAttribute.NotInherited;
						}
					}
					if (inheritanceAttribute.InheritanceLevel != InheritanceLevel.InheritedReadOnly)
					{
						flag = true;
					}
					break;
				}
				case CodeDomLocalizationModel.PropertyReflection:
					if (!flag)
					{
						if (this.localizationLanguage == null)
						{
							manager.SerializationComplete += this.OnSerializationComplete;
						}
						if (this.GetLocalizationLanguage(manager) != CultureInfo.InvariantCulture)
						{
							flag = true;
						}
					}
					break;
				}
			}
			return flag;
		}

		private CodeDomLocalizationModel _model;

		private MemberCodeDomSerializer _serializer;

		private CodeDomLocalizationProvider.LanguageExtenders _extender;

		private CultureInfo localizationLanguage;
	}
}
 
