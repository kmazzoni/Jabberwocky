﻿using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Autofac.Mvc.Models
{
	public abstract class GlassViewModel<TGlassModel> : InjectableGlassViewModelBase where TGlassModel : class, IGlassBase
	{
		public TGlassModel GlassModel => InternalModel as TGlassModel;
	}
}
