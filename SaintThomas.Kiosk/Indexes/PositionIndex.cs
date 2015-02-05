using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using SaintThomas.Kiosk.Models;

namespace SaintThomas.Kiosk.Indexes
{
    public class PositionIndex : AbstractIndexCreationTask<Image>
    {
        public PositionIndex()
        {
            Map = images => from image in images
                            select new
                            {
                                image.Position
                            };
            Sort(x => x.Position, SortOptions.Long);
        }
    }
}