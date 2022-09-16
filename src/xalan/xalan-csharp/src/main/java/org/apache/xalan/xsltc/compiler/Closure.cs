﻿/*
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
 * $Id: Closure.java 468650 2006-10-28 07:03:30Z minchau $
 */

namespace org.apache.xalan.xsltc.compiler
{
	/// <summary>
	/// @author Santiago Pericas-Geertsen
	/// </summary>
	public interface Closure
	{

		/// <summary>
		/// Returns true if this closure is compiled in an inner class (i.e.
		/// if this is a real closure).
		/// </summary>
		bool inInnerClass();

		/// <summary>
		/// Returns a reference to its parent closure or null if outermost.
		/// </summary>
		Closure ParentClosure {get;}

		/// <summary>
		/// Returns the name of the auxiliary class or null if this predicate 
		/// is compiled inside the Translet.
		/// </summary>
		string InnerClassName {get;}

		/// <summary>
		/// Add new variable to the closure.
		/// </summary>
		void addVariable(VariableRefBase variableRef);
	}

}