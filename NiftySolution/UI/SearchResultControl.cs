// Copyright (C) 2006-2009 Jim Tilander. See COPYING for and README for more details.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.IO;

namespace Aurora.NiftySolution
{
	public partial class SearchResultControl : UserControl
	{
		private string mQueryString = "";
		private int mHighlighted = -1;
		private SolutionFiles mFiles = null;
		private int mBorder = 3;

		public SolutionFiles SearchSpace
		{
			set { mFiles = value; }
		}

		public int CandidateCount
		{
			get { return mFiles.CandidateCount; }
		}

		public SearchEntry Selected
		{
			get
			{
				if(mFiles.CandidateCount == 0)
					return new SearchEntry();
				if(mHighlighted == -1)
					return mFiles.Candidate(0);
				return mFiles.Candidate(mHighlighted);
			}
		}

		public string QueryString
		{
			set
			{
				bool incremental = 1 < value.Length && mQueryString.Length < value.Length;

				mQueryString = value;
				UpdateSearchQuery(incremental);
			}
		}

		public SearchResultControl()
		{
			InitializeComponent();
		}

		private void SearchResultControl_Paint(object sender, PaintEventArgs e)
		{
			if(DesignMode == true)
				return;

			Graphics graphics = e.Graphics;
			graphics.ResetClip();

			graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;

			int rowHeight = Font.Height + mBorder;
			int maxRows = ClientRectangle.Height / rowHeight;

			Rectangle drawArea = Rectangle.Inflate(ClientRectangle, -mBorder, -mBorder);

			int lines = Math.Min(maxRows, mFiles.CandidateCount);

			for(int i = 0; i < lines; i++)
			{
				string lineText = Path.GetFileName(mFiles.Candidate(i).fullPath);

				RectangleF layoutRect = new RectangleF(drawArea.Left, drawArea.Top + i * rowHeight, drawArea.Width, rowHeight);


				if(i == mHighlighted)
				{
					
					//graphics.FillRectangle(Brushes.AliceBlue, layoutRect);
					graphics.FillRectangle(Brushes.DeepSkyBlue, layoutRect);
					RectangleF outline = new RectangleF(layoutRect.X, layoutRect.Y, layoutRect.Width - 1, layoutRect.Height - 1);
					graphics.DrawRectangle(Pens.Silver, Rectangle.Round(outline));
				}

				graphics.DrawString(lineText, Font, Brushes.Black, layoutRect);
			}
		}

		public void NextMatch()
		{
			mHighlighted += 1; 
			ValidateHighlightIndex();
			Invalidate();
		}

		public void PrevMatch()
		{
			mHighlighted -= 1;
			ValidateHighlightIndex();
			Invalidate();
		}

		private void UpdateSearchQuery(bool incrementalSearch)
		{
			mFiles.UpdateSearchQuery(mQueryString, incrementalSearch);

			if(mFiles.CandidateCount == 0)
			{
				mHighlighted = -1;
			}
			else if(mFiles.CandidateCount < mHighlighted)
			{
				mHighlighted = mFiles.CandidateCount - 1;
			}

			Invalidate();
		}

		private void SearchResultControl_MouseClick(object sender, MouseEventArgs e)
		{
			int rowHeight = Font.Height + mBorder;
			mHighlighted = (e.Y - mBorder) / rowHeight;
			if(!ValidateHighlightIndex())
				mHighlighted = -1;
			Invalidate();
		}

		private bool ValidateHighlightIndex()
		{
			if(mFiles.CandidateCount == 0)
			{
				mHighlighted = -1;
				return false;
			}
			if(mHighlighted < 0)
			{
				mHighlighted = mFiles.CandidateCount - 1;
				return false;
			}
			if(mHighlighted >= mFiles.CandidateCount)
			{
				mHighlighted = 0;
				return false;
			}
			return true;
		}
	}
}
