﻿using System;
using System.Collections;

/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership. The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the  "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
/*
 * $Id: FuncPosition.java 468655 2006-10-28 07:12:06Z minchau $
 */
namespace org.apache.xpath.functions
{
	using DTM = org.apache.xml.dtm.DTM;
	using DTMIterator = org.apache.xml.dtm.DTMIterator;
	using XPathContext = org.apache.xpath.XPathContext;
	using SubContextList = org.apache.xpath.axes.SubContextList;
	using Compiler = org.apache.xpath.compiler.Compiler;
	using XNumber = org.apache.xpath.objects.XNumber;
	using XObject = org.apache.xpath.objects.XObject;

	/// <summary>
	/// Execute the Position() function.
	/// @xsl.usage advanced
	/// </summary>
	[Serializable]
	public class FuncPosition : Function
	{
		internal new const long serialVersionUID = -9092846348197271582L;
	  private bool m_isTopLevel;

	  /// <summary>
	  /// Figure out if we're executing a toplevel expression.
	  /// If so, we can't be inside of a predicate. 
	  /// </summary>
	  public override void postCompileStep(Compiler compiler)
	  {
		m_isTopLevel = compiler.LocationPathDepth == -1;
	  }

	  /// <summary>
	  /// Get the position in the current context node list.
	  /// </summary>
	  /// <param name="xctxt"> Runtime XPath context.
	  /// </param>
	  /// <returns> The current position of the itteration in the context node list, 
	  ///         or -1 if there is no active context node list. </returns>
	  public virtual int getPositionInContextNodeList(XPathContext xctxt)
	  {

		// System.out.println("FuncPosition- entry");
		// If we're in a predicate, then this will return non-null.
		SubContextList iter = m_isTopLevel ? null : xctxt.SubContextList;

		if (null != iter)
		{
		  int prox = iter.getProximityPosition(xctxt);

		  // System.out.println("FuncPosition- prox: "+prox);
		  return prox;
		}

		DTMIterator cnl = xctxt.ContextNodeList;

		if (null != cnl)
		{
		  int n = cnl.CurrentNode;
		  if (n == DTM.NULL)
		  {
			if (cnl.CurrentPos == 0)
			{
			  return 0;
			}

			// Then I think we're in a sort.  See sort21.xsl. So the iterator has 
			// already been spent, and is not on the node we're processing. 
			// It's highly possible that this is an issue for other context-list 
			// functions.  Shouldn't be a problem for last(), and it shouldn't be 
			// a problem for current().
			try
			{
			  cnl = cnl.cloneWithReset();
			}
			catch (CloneNotSupportedException cnse)
			{
			  throw new org.apache.xml.utils.WrappedRuntimeException(cnse);
			}
			int currentNode = xctxt.ContextNode;
			// System.out.println("currentNode: "+currentNode);
			while (DTM.NULL != (n = cnl.nextNode()))
			{
			  if (n == currentNode)
			  {
				break;
			  }
			}
		  }
		  // System.out.println("n: "+n);
		  // System.out.println("FuncPosition- cnl.getCurrentPos(): "+cnl.getCurrentPos());
		  return cnl.CurrentPos;
		}

		// System.out.println("FuncPosition - out of guesses: -1");
		return -1;
	  }

	  /// <summary>
	  /// Execute the function.  The function must return
	  /// a valid object. </summary>
	  /// <param name="xctxt"> The current execution context. </param>
	  /// <returns> A valid XObject.
	  /// </returns>
	  /// <exception cref="javax.xml.transform.TransformerException"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public org.apache.xpath.objects.XObject execute(org.apache.xpath.XPathContext xctxt) throws javax.xml.transform.TransformerException
	  public override XObject execute(XPathContext xctxt)
	  {
		double pos = (double) getPositionInContextNodeList(xctxt);

		return new XNumber(pos);
	  }

	  /// <summary>
	  /// No arguments to process, so this does nothing.
	  /// </summary>
	  public override void fixupVariables(ArrayList vars, int globalsSize)
	  {
		// no-op
	  }
	}

}