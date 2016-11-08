using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using GeoJSON.Net;
using GeoJSON.Net.Geometry;

namespace Tecnomapas.EtramiteX.Scheduler.misc.WKT
{
	//Adapted from Converter.GeometryFromWKT Class from SharpMap 0.9.0 - http://sharpmap.codeplex.com
	public static class GeoJSONFromWKT
	{
		public static GeoJSONObject Parse(string wktString)
		{
			var reader = new StringReader(wktString);
			return Parse(reader);
		}

		public static GeoJSONObject Parse(TextReader reader)
		{
			var tokenizer = new WktStreamTokenizer(reader);
			return ReadGeometryTaggedText(tokenizer);
		}

		private static List<double> GetCoordinates(WktStreamTokenizer tokenizer)
		{
			var coordinates = new List<double>();
			var nextToken = GetNextEmptyOrOpener(tokenizer);
			if (nextToken == "EMPTY")
				return coordinates;

			var longitude = GetNextNumber(tokenizer);
			var latitude = GetNextNumber(tokenizer);

			//longitude = longitude.Replace(",", ".");
			//latitude = latitude.Replace(",", ".");
			coordinates.Add(longitude);
			coordinates.Add(latitude);
			nextToken = GetNextCloserOrComma(tokenizer);

			while (nextToken == ",")
			{
				longitude = GetNextNumber(tokenizer);
				latitude = GetNextNumber(tokenizer);
				//longitude = longitude.Replace(",", ".");
				//latitude = latitude.Replace(",", ".");
				coordinates.Add(longitude);
				coordinates.Add(latitude);
				nextToken = GetNextCloserOrComma(tokenizer);
			}
			return coordinates;
		}

		private static double GetNextNumber(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();
			return tokenizer.GetNumericValue();
		}

		private static string GetNextEmptyOrOpener(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();
			string nextWord = tokenizer.GetStringValue();
			if (nextWord == "EMPTY" || nextWord == "(")
				return nextWord;

			throw new Exception("Expected 'EMPTY' or '(' but encountered '" + nextWord + "'");
		}

		private static string GetNextCloserOrComma(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();
			string nextWord = tokenizer.GetStringValue();
			if (nextWord == "," || nextWord == ")")
			{
				return nextWord;
			}
			throw new Exception("Expected ')' or ',' but encountered '" + nextWord + "'");
		}

		private static string GetNextCloser(WktStreamTokenizer tokenizer)
		{
			string nextWord = GetNextWord(tokenizer);
			if (nextWord == ")")
				return nextWord;

			throw new Exception("Expected ')' but encountered '" + nextWord + "'");
		}

		private static string GetNextWord(WktStreamTokenizer tokenizer)
		{
			TokenType type = tokenizer.NextToken();
			string token = tokenizer.GetStringValue();
			if (type == TokenType.Number)
				throw new Exception("Expected a number but got " + token);
			else if (type == TokenType.Word)
				return token.ToUpper();
			else if (token == "(")
				return "(";
			else if (token == ")")
				return ")";
			else if (token == ",")
				return ",";

			throw new Exception("Not a valid symbol in WKT format.");
		}

		private static GeoJSONObject ReadGeometryTaggedText(WktStreamTokenizer tokenizer)
		{
			tokenizer.NextToken();
			var type = tokenizer.GetStringValue().ToUpper();
			GeoJSONObject geometry = null;
			switch (type)
			{
				case "POINT":
					geometry = ReadPointText(tokenizer);
					break;
				case "LINESTRING":
					geometry = ReadLineStringText(tokenizer);
					break;
				/*case "MULTIPOINT":
					geometry = ReadMultiPointText(tokenizer);
					break;
				case "MULTILINESTRING":
					geometry = ReadMultiLineStringText(tokenizer);
					break;*/
				case "POLYGON":
					geometry = ReadPolygonText(tokenizer);
					break;
				/*case "MULTIPOLYGON":
					geometry = ReadMultiPolygonText(tokenizer);
					break;
				case "GEOMETRYCOLLECTION":
					geometry = ReadGeometryCollectionText(tokenizer);
					break;*/
				default:
					throw new Exception(String.Format(new CultureInfo("en-US", false).NumberFormat, "Geometrytype '{0}' is not supported.",
													  type));
			}
			return geometry;
		}
		/*
		private static CellGeom ReadMultiPolygonText(WktStreamTokenizer tokenizer)
		{
			CellGeom polygons = new List<List<Coordinate>>();
			string nextToken = GetNextEmptyOrOpener(tokenizer);
			if (nextToken == "EMPTY")
				return polygons;

			List<Coordinate> polygon = ReadPolygonText(tokenizer);
			polygons.Add(polygon);
			nextToken = GetNextCloserOrComma(tokenizer);
			while (nextToken == ",")
			{
				polygon = ReadPolygonText(tokenizer);
				polygons.Add(polygon);
				nextToken = GetNextCloserOrComma(tokenizer);
			}
			return polygons;
		}*/

		private static Polygon ReadPolygonText(WktStreamTokenizer tokenizer)
		{
			//var pol = new Polygon(new List<LineString>
			//{
			//	new LineString(new List<GeographicPosition>()
			//	{
			//		new GeographicPosition(0,0),
			//		new GeographicPosition(1,1),
			//		new GeographicPosition(0,0)
			//	})
			//});

			var nextToken = GetNextEmptyOrOpener(tokenizer);
			if (nextToken == "EMPTY")
				return null;

			var aneis = new List<LineString>();

			do
			{
				var ring = GetCoordinates(tokenizer);

				var max = ring.Count;

				var vertices = new List<GeographicPosition>();

				for (var i = 0; i < max; i++)
				{
					var lng = ring[i];
					var lat = ring[i + 1];

					var coord = new GeographicPosition(lat, lng);

					i++;
					vertices.Add(coord);
				}

				aneis.Add(new LineString(vertices));

				nextToken = GetNextCloserOrComma(tokenizer);
			} while (nextToken == ",");

			var pol = new Polygon(
				new List<LineString>(aneis)
			);
			
			return pol;
		}

		private static Point ReadPointText(WktStreamTokenizer tokenizer)
		{
			var p = new Point(new GeographicPosition(0,0));

			var nextToken = GetNextEmptyOrOpener(tokenizer);
			if (nextToken == "EMPTY")
				return p;

			var longitude = GetNextNumber(tokenizer);
			var latitude = GetNextNumber(tokenizer);
			
			p = new Point(new GeographicPosition(latitude, longitude));
			
			GetNextCloser(tokenizer);

			return p;
		}
		/*
		private static CellGeom ReadMultiPointText(WktStreamTokenizer tokenizer)
		{
			CellGeom mp = new CellGeom();
			mp.coordinates = new List<List<Coordinate>>();

			List<Coordinate> ring = new List<Coordinate>();

			string nextToken = GetNextEmptyOrOpener(tokenizer);
			if (nextToken == "EMPTY")
				return mp;

			Coordinate coord = new Coordinate();
			coord.lng = Convert.ToString(GetNextNumber(tokenizer));
			coord.lat = Convert.ToString(GetNextNumber(tokenizer));
			ring.Add(coord);
			mp.coordinates.Add(ring);
			
			nextToken = GetNextCloserOrComma(tokenizer);
			while (nextToken == ",")
			{
				List<Coordinate> newRing = new List<Coordinate>();
				Coordinate newCoord = new Coordinate();
				newCoord.lng = Convert.ToString(GetNextNumber(tokenizer));
				newCoord.lat = Convert.ToString(GetNextNumber(tokenizer));
				newRing.Add(newCoord);
				mp.coordinates.Add(newRing);
				
				nextToken = GetNextCloserOrComma(tokenizer);
			}
			return mp;
		}*/

		/*private static CellGeom ReadMultiLineStringText(WktStreamTokenizer tokenizer)
		{
			CellGeom lines = new CellGeom();
			string nextToken = GetNextEmptyOrOpener(tokenizer);
			if (nextToken == "EMPTY")
				return lines;

			lines.LineStrings.Add(ReadLineStringText(tokenizer));
			nextToken = GetNextCloserOrComma(tokenizer);
			while (nextToken == ",")
			{
				lines.LineStrings.Add(ReadLineStringText(tokenizer));
				nextToken = GetNextCloserOrComma(tokenizer);
			}
			return lines;
		}*/

		private static GeoJSONObject ReadLineStringText(WktStreamTokenizer tokenizer)
		{
			var ring = GetCoordinates(tokenizer);

			var max = ring.Count;

			var vertices = new List<GeographicPosition>();

			for (var i = 0; i < max; i++)
			{
				var lng = ring[i];
				var lat = ring[i + 1];

				var coord = new GeographicPosition(lat, lng);

				i++;
				vertices.Add(coord);
			}

			var line = new LineString(vertices);
			
			return line;
		}
		/*
		private static GeometryCollection ReadGeometryCollectionText(WktStreamTokenizer tokenizer)
		{
			GeometryCollection geometries = new GeometryCollection();
			string nextToken = GetNextEmptyOrOpener(tokenizer);
			if (nextToken.Equals("EMPTY"))
				return geometries;
			geometries.Collection.Add(ReadGeometryTaggedText(tokenizer));
			nextToken = GetNextCloserOrComma(tokenizer);
			while (nextToken.Equals(","))
			{
				geometries.Collection.Add(ReadGeometryTaggedText(tokenizer));
				nextToken = GetNextCloserOrComma(tokenizer);
			}
			return geometries;
		}*/
	}
}
