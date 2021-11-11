using System;
using System.Collections;
using System.Design;
using System.Globalization;
using System.Resources;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design.Serialization
{
	/// <summary>Provides CodeDOM resource serialization services. This class cannot be inherited.</summary>
	public sealed class CodeDomLocalizationProvider : IDisposable, IDesignerSerializationProvider
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.Serialization.CodeDomLocalizationProvider" /> class. </summary>
		/// <param name="provider">An <see cref="T:System.IServiceProvider" /> used by the localization provider to add its extender properties.</param>
		/// <param name="model">A <see cref="T:System.ComponentModel.Design.Serialization.CodeDomLocalizationModel" /> value indicating the localization model to be used by the CodeDOM resource adapter </param>
		public CodeDomLocalizationProvider(IServiceProvider provider, CodeDomLocalizationModel model)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			this._model = model;
			this.Initialize(provider);
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.Design.Serialization.CodeDomLocalizationProvider" /> class. </summary>
		/// <param name="provider">An <see cref="T:System.IServiceProvider" /> used by the localization provider to add its extender properties.</param>
		/// <param name="model">A <see cref="T:System.ComponentModel.Design.Serialization.CodeDomLocalizationModel" /> value indicating the localization model to be used by the CodeDOM resource adapter </param>
		/// <param name="supportedCultures">An array of cultures that this resource adapter should support.</param>
		public CodeDomLocalizationProvider(IServiceProvider provider, CodeDomLocalizationModel model, CultureInfo[] supportedCultures)
		{
			if (provider == null)
			{
				throw new ArgumentNullException("provider");
			}
			if (supportedCultures == null)
			{
				throw new ArgumentNullException("supportedCultures");
			}
			this._model = model;
			this._supportedCultures = (CultureInfo[])supportedCultures.Clone();
			this.Initialize(provider);
		}

		/// <summary>Releases all resources used by the <see cref="T:System.ComponentModel.Design.Serialization.CodeDomLocalizationProvider" />.</summary>
		public void Dispose()
		{
			if (this._providerService != null && this._extender != null)
			{
				this._providerService.RemoveExtenderProvider(this._extender);
				this._providerService = null;
				this._extender = null;
			}
		}

		private void Initialize(IServiceProvider provider)
		{
			this._providerService = (provider.GetService(typeof(IExtenderProviderService)) as IExtenderProviderService);
			if (this._providerService == null)
			{
				throw new NotSupportedException(SR.GetString("LocalizationProviderMissingService", new object[]
				{
					typeof(IExtenderProviderService).Name
				}));
			}
			this._extender = new CodeDomLocalizationProvider.LanguageExtenders(provider, this._supportedCultures);
			this._providerService.AddExtenderProvider(this._extender);
		}

		private object GetCodeDomSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
		{
			if (currentSerializer == null)
			{
				return null;
			}
			if (typeof(ResourceManager).IsAssignableFrom(objectType))
			{
				return null;
			}
			CodeDomLocalizationModel codeDomLocalizationModel = CodeDomLocalizationModel.None;
			object obj = manager.Context[typeof(CodeDomLocalizationModel)];
			if (obj != null)
			{
				codeDomLocalizationModel = (CodeDomLocalizationModel)obj;
			}
			if (codeDomLocalizationModel != CodeDomLocalizationModel.None)
			{
				return new LocalizationCodeDomSerializer(codeDomLocalizationModel, currentSerializer);
			}
			return null;
		}

		private object GetMemberCodeDomSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
		{
			CodeDomLocalizationModel codeDomLocalizationModel = this._model;
			if (!typeof(PropertyDescriptor).IsAssignableFrom(objectType))
			{
				return null;
			}
			if (currentSerializer == null)
			{
				return null;
			}
			if (currentSerializer is ResourcePropertyMemberCodeDomSerializer)
			{
				return null;
			}
			if (this._extender == null || !this._extender.GetLocalizable(null))
			{
				return null;
			}
			PropertyDescriptor propertyDescriptor = manager.Context[typeof(PropertyDescriptor)] as PropertyDescriptor;
			if (propertyDescriptor == null || !propertyDescriptor.IsLocalizable)
			{
				codeDomLocalizationModel = CodeDomLocalizationModel.None;
			}
			if (this._memberSerializers == null)
			{
				this._memberSerializers = new Hashtable();
			}
			if (this._nopMemberSerializers == null)
			{
				this._nopMemberSerializers = new Hashtable();
			}
			object obj;
			if (codeDomLocalizationModel == CodeDomLocalizationModel.None)
			{
				obj = this._nopMemberSerializers[currentSerializer];
			}
			else
			{
				obj = this._memberSerializers[currentSerializer];
			}
			if (obj == null)
			{
				obj = new ResourcePropertyMemberCodeDomSerializer((MemberCodeDomSerializer)currentSerializer, this._extender, codeDomLocalizationModel);
				if (codeDomLocalizationModel == CodeDomLocalizationModel.None)
				{
					this._nopMemberSerializers[currentSerializer] = obj;
				}
				else
				{
					this._memberSerializers[currentSerializer] = obj;
				}
			}
			return obj;
		}

		/// <summary>For a description of this member, see <see cref="M:System.ComponentModel.Design.Serialization.IDesignerSerializationProvider.GetSerializer(System.ComponentModel.Design.Serialization.IDesignerSerializationManager,System.Object,System.Type,System.Type)" />.</summary>
		/// <returns>An instance of a serializer of the type requested, or null if the request cannot be satisfied.</returns>
		/// <param name="manager">The serialization manager requesting the serializer. </param>
		/// <param name="currentSerializer">An instance of the current serializer of the specified type. This can be null if no serializer of the specified type exists. </param>
		/// <param name="objectType">The data type of the object to serialize. </param>
		/// <param name="serializerType">The data type of the serializer to create. </param>
		object IDesignerSerializationProvider.GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
		{
			if (serializerType == typeof(CodeDomSerializer))
			{
				return this.GetCodeDomSerializer(manager, currentSerializer, objectType, serializerType);
			}
			if (serializerType == typeof(MemberCodeDomSerializer))
			{
				return this.GetMemberCodeDomSerializer(manager, currentSerializer, objectType, serializerType);
			}
			return null;
		}

		private IExtenderProviderService _providerService;

		private CodeDomLocalizationModel _model;

		private CultureInfo[] _supportedCultures;

		private CodeDomLocalizationProvider.LanguageExtenders _extender;

		private Hashtable _memberSerializers;

		private Hashtable _nopMemberSerializers;

		[ProvideProperty("Language", typeof(IComponent))]
		[ProvideProperty("LoadLanguage", typeof(IComponent))]
		[ProvideProperty("Localizable", typeof(IComponent))]
		internal class LanguageExtenders : IExtenderProvider
		{
			public LanguageExtenders(IServiceProvider serviceProvider, CultureInfo[] supportedCultures)
			{
				this._serviceProvider = serviceProvider;
				this._host = (serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost);
				this._language = CultureInfo.InvariantCulture;
				if (supportedCultures != null)
				{
					this._supportedCultures = new TypeConverter.StandardValuesCollection(supportedCultures);
				}
			}

			internal TypeConverter.StandardValuesCollection SupportedCultures
			{
				get
				{
					return this._supportedCultures;
				}
			}

			private CultureInfo ThreadDefaultLanguage
			{
				get
				{
					if (this._defaultLanguage == null)
					{
						this._defaultLanguage = Application.CurrentCulture;
					}
					return this._defaultLanguage;
				}
			}

			private void BroadcastGlobalChange(IComponent comp)
			{
				ISite site = comp.Site;
				if (site != null)
				{
					IComponentChangeService componentChangeService = site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
					IContainer container = site.GetService(typeof(IContainer)) as IContainer;
					if (componentChangeService != null && container != null)
					{
						foreach (object obj in container.Components)
						{
							IComponent component = (IComponent)obj;
							componentChangeService.OnComponentChanging(component, null);
							componentChangeService.OnComponentChanged(component, null, null, null);
						}
					}
				}
			}

			private void CheckRoot()
			{
				if (this._host != null && this._host.RootComponent != this._lastRoot)
				{
					this._lastRoot = this._host.RootComponent;
					this._language = CultureInfo.InvariantCulture;
					this._loadLanguage = null;
					this._localizable = false;
				}
			}

			[DesignOnly(true)]
			[TypeConverter(typeof(CodeDomLocalizationProvider.LanguageCultureInfoConverter))]
			[Category("Design")]
			//[SRDescription("LocalizationProviderLanguageDescr")]
			public CultureInfo GetLanguage(IComponent o)
			{
				this.CheckRoot();
				return this._language;
			}

			[Browsable(false)]
			[DesignOnly(true)]
			[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public CultureInfo GetLoadLanguage(IComponent o)
			{
				this.CheckRoot();
				if (this._loadLanguage == null)
				{
					this._loadLanguage = CultureInfo.InvariantCulture;
				}
				return this._loadLanguage;
			}

			[DesignOnly(true)]
			[Category("Design")]
			//[SRDescription("LocalizationProviderLocalizableDescr")]
			public bool GetLocalizable(IComponent o)
			{
				this.CheckRoot();
				return this._localizable;
			}

			public void SetLanguage(IComponent o, CultureInfo language)
			{
				this.CheckRoot();
				if (language == null)
				{
					language = CultureInfo.InvariantCulture;
				}
				bool flag = language.Equals(CultureInfo.InvariantCulture);
				CultureInfo threadDefaultLanguage = this.ThreadDefaultLanguage;
				if (this._language.Equals(language))
				{
					return;
				}
				this._language = language;
				if (!flag)
				{
					this.SetLocalizable(o, true);
				}
				if (this._serviceProvider != null && this._host != null)
				{
					IDesignerLoaderService designerLoaderService = this._serviceProvider.GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;
					if (this._host.Loading)
					{
						this._loadLanguage = language;
						return;
					}
					bool flag2 = false;
					if (designerLoaderService != null)
					{
						flag2 = designerLoaderService.Reload();
					}
					if (!flag2)
					{
						IUIService iuiservice = (IUIService)this._serviceProvider.GetService(typeof(IUIService));
						if (iuiservice != null)
						{
							iuiservice.ShowMessage(SR.GetString("LocalizationProviderManualReload"));
						}
					}
				}
			}

			public void SetLocalizable(IComponent o, bool localizable)
			{
				this.CheckRoot();
				if (localizable != this._localizable)
				{
					this._localizable = localizable;
					if (!localizable)
					{
						this.SetLanguage(o, CultureInfo.InvariantCulture);
					}
					if (this._host != null && !this._host.Loading)
					{
						this.BroadcastGlobalChange(o);
					}
				}
			}

			private bool ShouldSerializeLanguage(IComponent o)
			{
				return this._language != null && this._language != CultureInfo.InvariantCulture;
			}

			private bool ShouldSerializeLocalizable(IComponent o)
			{
				return this._localizable;
			}

			private void ResetLocalizable(IComponent o)
			{
				this.SetLocalizable(o, false);
			}

			private void ResetLanguage(IComponent o)
			{
				this.SetLanguage(o, CultureInfo.InvariantCulture);
			}

			public bool CanExtend(object o)
			{
				this.CheckRoot();
				return this._host != null && o == this._host.RootComponent;
			}

			private IServiceProvider _serviceProvider;

			private IDesignerHost _host;

			private IComponent _lastRoot;

			private TypeConverter.StandardValuesCollection _supportedCultures;

			private bool _localizable;

			private CultureInfo _language;

			private CultureInfo _loadLanguage;

			private CultureInfo _defaultLanguage;
		}

		internal sealed class LanguageCultureInfoConverter : CultureInfoConverter
		{
			public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				TypeConverter.StandardValuesCollection standardValuesCollection = null;
				if (context.PropertyDescriptor != null)
				{
					ExtenderProvidedPropertyAttribute extenderProvidedPropertyAttribute = context.PropertyDescriptor.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
					if (extenderProvidedPropertyAttribute != null)
					{
						CodeDomLocalizationProvider.LanguageExtenders languageExtenders = extenderProvidedPropertyAttribute.Provider as CodeDomLocalizationProvider.LanguageExtenders;
						if (languageExtenders != null)
						{
							standardValuesCollection = languageExtenders.SupportedCultures;
						}
					}
				}
				if (standardValuesCollection == null)
				{
					standardValuesCollection = base.GetStandardValues(context);
				}
				return standardValuesCollection;
			}
		}
	}
}
