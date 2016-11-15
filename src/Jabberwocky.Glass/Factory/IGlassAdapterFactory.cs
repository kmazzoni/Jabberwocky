using System.Collections.Generic;
using Jabberwocky.Glass.Models;

namespace Jabberwocky.Glass.Factory
{
	public interface IGlassAdapterFactory
	{
		T GetItem<T>(IGlassBase model) where T : class;
		IEnumerable<T> GetItems<T>(IEnumerable<IGlassBase> models) where T : class;
	}
}
