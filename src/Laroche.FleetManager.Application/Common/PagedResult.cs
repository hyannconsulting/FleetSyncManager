using System;
using System.Collections.Generic;

namespace Laroche.FleetManager.Application.Common
{
    /// <summary>
    /// Résultat paginé pour les listes
    /// </summary>
    /// <typeparam name="T">Type des éléments</typeparam>
    public class PagedResult<T>
    {
        /// <summary>
        /// Liste des éléments de la page courante
        /// </summary>
        public IEnumerable<T> Items { get; set; } = new List<T>();

        /// <summary>
        /// Numéro de la page courante (1-based)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Nombre d'éléments par page
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Nombre total d'éléments
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Nombre total de pages
        /// </summary>
        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;

        /// <summary>
        /// Indique s'il y a une page précédente
        /// </summary>
        public bool HasPreviousPage => Page > 1;

        /// <summary>
        /// Indique s'il y a une page suivante
        /// </summary>
        public bool HasNextPage => Page < TotalPages;

        /// <summary>
        /// Constructeur par défaut
        /// </summary>
        public PagedResult()
        {
        }

        /// <summary>
        /// Constructeur avec paramètres
        /// </summary>
        /// <param name="items">Éléments de la page</param>
        /// <param name="page">Numéro de page</param>
        /// <param name="pageSize">Taille de page</param>
        /// <param name="totalCount">Nombre total d'éléments</param>
        public PagedResult(IEnumerable<T> items, int page, int pageSize, int totalCount)
        {
            Items = items ?? new List<T>();
            Page = Math.Max(1, page);
            PageSize = Math.Max(1, pageSize);
            TotalCount = Math.Max(0, totalCount);
        }

        /// <summary>
        /// Crée un résultat paginé vide
        /// </summary>
        /// <param name="page">Numéro de page</param>
        /// <param name="pageSize">Taille de page</param>
        /// <returns>Résultat vide</returns>
        public static PagedResult<T> Empty(int page = 1, int pageSize = 10)
        {
            return new PagedResult<T>(new List<T>(), page, pageSize, 0);
        }

        /// <summary>
        /// Crée un résultat paginé avec des données
        /// </summary>
        /// <param name="items">Éléments</param>
        /// <param name="page">Numéro de page</param>
        /// <param name="pageSize">Taille de page</param>
        /// <param name="totalCount">Nombre total</param>
        /// <returns>Résultat paginé</returns>
        public static PagedResult<T> Create(IEnumerable<T> items, int page, int pageSize, int totalCount)
        {
            return new PagedResult<T>(items, page, pageSize, totalCount);
        }
    }
}
