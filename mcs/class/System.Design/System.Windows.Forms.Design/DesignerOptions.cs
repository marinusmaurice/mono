using System;
using System.ComponentModel;
using System.Design;
using System.Drawing;

namespace System.Windows.Forms.Design
{
	/// <summary>Provides access to get and set option values for a designer.</summary>
	// Token: 0x0200021E RID: 542
	public class DesignerOptions
	{
		/// <summary>Gets or sets a <see cref="T:System.Drawing.Size" /> representing the dimensions of a grid unit. </summary>
		/// <returns>A <see cref="T:System.Drawing.Size" /> representing the dimensions of a grid unit.</returns>
		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06001455 RID: 5205 RVA: 0x00067449 File Offset: 0x00066449
		// (set) Token: 0x06001456 RID: 5206 RVA: 0x00067454 File Offset: 0x00066454
	 
		public virtual Size GridSize
		{
			get
			{
				return this.gridSize;
			}
			set
			{
				if (value.Width < 2)
				{
					value.Width = 2;
				}
				if (value.Height < 2)
				{
					value.Height = 2;
				}
				if (value.Width > 200)
				{
					value.Width = 200;
				}
				if (value.Height > 200)
				{
					value.Height = 200;
				}
				this.gridSize = value;
			}
		}

		/// <summary>Gets or sets a value that enables or disables the grid in the designer. </summary>
		/// <returns>true if the grid is enabled; otherwise, false. The default is true.</returns>
		// Token: 0x17000343 RID: 835
		// (get) Token: 0x06001457 RID: 5207 RVA: 0x000674C0 File Offset: 0x000664C0
		// (set) Token: 0x06001458 RID: 5208 RVA: 0x000674C8 File Offset: 0x000664C8
	 
		public virtual bool ShowGrid
		{
			get
			{
				return this.showGrid;
			}
			set
			{
				this.showGrid = value;
			}
		}

		/// <summary>Gets or sets a value that enables or disables whether controls are automatically placed at grid coordinates. </summary>
		/// <returns>true if snapping is enabled; otherwise, false.</returns>
		// Token: 0x17000344 RID: 836
		// (get) Token: 0x06001459 RID: 5209 RVA: 0x000674D1 File Offset: 0x000664D1
		// (set) Token: 0x0600145A RID: 5210 RVA: 0x000674D9 File Offset: 0x000664D9
	 
		public virtual bool SnapToGrid
		{
			get
			{
				return this.snapToGrid;
			}
			set
			{
				this.snapToGrid = value;
			}
		}

		/// <summary>Gets or sets a value that enables or disables snaplines in the designer.</summary>
		/// <returns>true if snaplines in the designer are enabled; otherwise, false.</returns>
		// Token: 0x17000345 RID: 837
		// (get) Token: 0x0600145B RID: 5211 RVA: 0x000674E2 File Offset: 0x000664E2
		// (set) Token: 0x0600145C RID: 5212 RVA: 0x000674EA File Offset: 0x000664EA
	 
		public virtual bool UseSnapLines
		{
			get
			{
				return this.useSnapLines;
			}
			set
			{
				this.useSnapLines = value;
			}
		}

		/// <summary>Gets or sets a value that enables or disables smart tags in the designer.</summary>
		/// <returns>true if smart tags in the designer are enabled; otherwise, false.</returns>
		// Token: 0x17000346 RID: 838
		// (get) Token: 0x0600145D RID: 5213 RVA: 0x000674F3 File Offset: 0x000664F3
		// (set) Token: 0x0600145E RID: 5214 RVA: 0x000674FB File Offset: 0x000664FB
	 
		public virtual bool UseSmartTags
		{
			get
			{
				return this.useSmartTags;
			}
			set
			{
				this.useSmartTags = value;
			}
		}

		/// <summary>Gets or sets a value that specifies whether a designer shows a component's smart tag panel automatically on creation. </summary>
		/// <returns>true to allow a component's smart tag panel to open automatically upon creation; otherwise, false. The default is true.</returns>
		// Token: 0x17000347 RID: 839
		// (get) Token: 0x0600145F RID: 5215 RVA: 0x00067504 File Offset: 0x00066504
		// (set) Token: 0x06001460 RID: 5216 RVA: 0x0006750C File Offset: 0x0006650C
		 
		public virtual bool ObjectBoundSmartTagAutoShow
		{
			get
			{
				return this.objectBoundSmartTagAutoShow;
			}
			set
			{
				this.objectBoundSmartTagAutoShow = value;
			}
		}

		/// <summary>Gets or sets a value that enables or disables the component cache. </summary>
		/// <returns>true if the component cache is enabled; otherwise, false. The default is true.</returns>
		// Token: 0x17000348 RID: 840
		// (get) Token: 0x06001461 RID: 5217 RVA: 0x00067515 File Offset: 0x00066515
		// (set) Token: 0x06001462 RID: 5218 RVA: 0x0006751D File Offset: 0x0006651D
		 
		public virtual bool UseOptimizedCodeGeneration
		{
			get
			{
				return this.enableComponentCache;
			}
			set
			{
				this.enableComponentCache = value;
			}
		}

		/// <summary>Gets or sets a value that enables or disables in-place editing for <see cref="T:System.Windows.Forms.ToolStrip" /> controls.</summary>
		/// <returns>true if in-place editing for <see cref="T:System.Windows.Forms.ToolStrip" /> controls is enabled; otherwise, false. The default is true.</returns>
		// Token: 0x17000349 RID: 841
		// (get) Token: 0x06001463 RID: 5219 RVA: 0x00067526 File Offset: 0x00066526
		// (set) Token: 0x06001464 RID: 5220 RVA: 0x0006752E File Offset: 0x0006652E
		 
		[Browsable(false)]
	 
		public virtual bool EnableInSituEditing
		{
			get
			{
				return this.enableInSituEditing;
			}
			set
			{
				this.enableInSituEditing = value;
			}
		}

		// Token: 0x040011FC RID: 4604
		private const int minGridSize = 2;

		// Token: 0x040011FD RID: 4605
		private const int maxGridSize = 200;

		// Token: 0x040011FE RID: 4606
		private bool showGrid = true;

		// Token: 0x040011FF RID: 4607
		private bool snapToGrid = true;

		// Token: 0x04001200 RID: 4608
		private Size gridSize = new Size(8, 8);

		// Token: 0x04001201 RID: 4609
		private bool useSnapLines;

		// Token: 0x04001202 RID: 4610
		private bool useSmartTags;

		// Token: 0x04001203 RID: 4611
		private bool objectBoundSmartTagAutoShow = true;

		// Token: 0x04001204 RID: 4612
		private bool enableComponentCache;

		// Token: 0x04001205 RID: 4613
		private bool enableInSituEditing = true;
	}
}
