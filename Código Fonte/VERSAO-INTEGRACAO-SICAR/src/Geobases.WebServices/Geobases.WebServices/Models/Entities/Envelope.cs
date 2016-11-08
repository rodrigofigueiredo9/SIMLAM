using System;
using System.Collections.Generic;
using System.Web;

namespace Tecnomapas.Geobases.WebServices.Models.Entities
{
	public class Envelope
	{
        private int _id;
        private string _nome;
        private decimal _minX;
        private decimal _minY;
        private decimal _maxX;
        private decimal _maxY;
        public decimal[][] Coordenadas { get; set; }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }
        public decimal MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }
        public decimal MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }
        public decimal MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }
        public decimal MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }
	}
}
