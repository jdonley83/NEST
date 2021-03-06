﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Nest
{
	public partial class ElasticClient
	{
		/// <summary>
		/// Synchronously search using dynamic as its return type.
		/// </summary>
		public IQueryResponse<dynamic> Search(Func<SearchDescriptor<dynamic>, SearchDescriptor<dynamic>> searcher)
		{
			var search = new SearchDescriptor<dynamic>();
			var descriptor = searcher(search);
			var path = this.PathResolver.GetSearchPathForDynamic(descriptor);
			var query = this.Serialize(descriptor);

			ConnectionStatus status = this.Connection.PostSync(path, query);
			var r = this.ToParsedResponse<QueryResponse<dynamic>>(status);
			return r;
		}

		/// <summary>
		/// Synchronously search using T as the return type
		/// </summary>
		public IQueryResponse<T> Search<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class
		{
			var search = new SearchDescriptor<T>();
			var descriptor = searcher(search);
			return Search(descriptor);
		}

		/// <summary>
		/// Synchronously search using T as the return type
		/// </summary>
		public IQueryResponse<T> Search<T>(SearchDescriptor<T> descriptor) where T : class
		{
			var query = this.Serialize(descriptor);
			var path = this.PathResolver.GetSearchPathForTyped(descriptor);
			var status = this.Connection.PostSync(path, query);
			return this.ToParsedResponse<QueryResponse<T>>(status);
		}

		/// <summary>
		/// Synchronously search using T as the return type, string based.
		/// </summary>
		/// <param name="path">EXPERT OPTION: Pass the path and querystring used to perform the search on, when null it will be infered from the type</param>
		public IQueryResponse<T> SearchRaw<T>(string query, string path = null) where T : class
		{
			var descriptor = new SearchDescriptor<T>();

			if (string.IsNullOrEmpty(path))
				path = this.PathResolver.GetSearchPathForTyped(descriptor);

			ConnectionStatus status = this.Connection.PostSync(path, query);
			var r = this.ToParsedResponse<QueryResponse<T>>(status);
			return r;
		}
		/// <summary>
		/// Asynchronously search using T as the return type, string based.
		/// </summary>
		/// <param name="path">EXPERT OPTION: Pass the path and querystring used to perform the search on, when null it will be infered from the type</param>
		public Task<IQueryResponse<T>> SearchRawAsync<T>(string query, string path = null) where T : class
		{
			var descriptor = new SearchDescriptor<T>();
			
			if (string.IsNullOrEmpty(path))
				path = this.PathResolver.GetSearchPathForTyped(descriptor);

			var task = this.Connection.Post(path, query);
			return task.ContinueWith<IQueryResponse<T>>(t => this.ToParsedResponse<QueryResponse<T>>(task.Result));
		}


		/// <summary>
		/// Asynchronously search using dynamic as its return type.
		/// </summary>
		public Task<IQueryResponse<dynamic>> SearchAsync(Func<SearchDescriptor<dynamic>, SearchDescriptor<dynamic>> searcher)
		{
			var search = new SearchDescriptor<dynamic>();
			var descriptor = searcher(search);
			var path = this.PathResolver.GetSearchPathForDynamic(descriptor);
			var query = this.Serialize(descriptor);

			var task = this.Connection.Post(path, query);
			return task.ContinueWith<IQueryResponse<dynamic>>(t => this.ToParsedResponse<QueryResponse<dynamic>>(t.Result));
		}

		/// <summary>
		/// Asynchronously search using T as the return type
		/// </summary>
		public Task<IQueryResponse<T>> SearchAsync<T>(Func<SearchDescriptor<T>, SearchDescriptor<T>> searcher) where T : class
		{
			var search = new SearchDescriptor<T>();
			var descriptor = searcher(search);
			return SearchAsync(descriptor);
		}

		/// <summary>
		/// Asynchronously search using T as the return type
		/// </summary>
		public Task<IQueryResponse<T>> SearchAsync<T>(SearchDescriptor<T> descriptor) where T : class
		{
			var query = this.Serialize(descriptor);
			var path = this.PathResolver.GetSearchPathForTyped(descriptor);

			var task = this.Connection.Post(path, query);
			return task.ContinueWith<IQueryResponse<T>>(t => this.ToParsedResponse<QueryResponse<T>>(task.Result));
		}

		
	}
}