﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nadia.C.Sharp.FactValueFolder;
using Nadia.C.Sharp.RuleParserFolder;

namespace Nadia.C.Sharp.NodeFolder
{
    public abstract class Node <T>
    {

        protected static int staticNodeId = 0;
        protected int nodeId;
        protected string nodeName;
        protected int nodeLine;
        protected string variableName;
        protected FactValue<T> value;
        protected Tokens tokens;

        public Node(string parentText, Tokens tokens)
        {
            nodeId = staticNodeId;
            staticNodeId++;
            this.tokens = tokens;

            Initialisation(parentText, tokens);
        }

        public abstract void Initialisation(string parentText, Tokens tokens);
        public abstract LineType GetLineType();
        public abstract FactValue<T> SelfEvaluate(Dictionary<string, FactValue<T>> workingMemory, Jint.Engine nashorn);

        public void SetNodeLine(int nodeLine)
        {
            this.nodeLine = nodeLine;
        }
        public int GetNodeLine()
        {
            return this.nodeLine;
        }

        public static int GetStaticNodeId()
        {
            return staticNodeId;
        }
        public int GetNodeId()
        {
            return this.nodeId;
        }
        public string GetNodeName()
        {
            return this.nodeName;
        }

        public Tokens GetTokens()
        {
            return this.tokens;
        }

        public string GetVariableName()
        {
            return variableName;
        }
        public void SetNodeVariable(string newVariableName)
        {
            this.variableName = newVariableName;
        }

        public FactValue<T> GetFactValue()
        {
            return this.value;
        }

        protected void SetValue(string lastTokenstring, string lastToken)
        {
            switch (lastTokenstring)
            {
                case "No":
                    int intValue = 0;
                    Int32.TryParse(lastToken, out intValue);

                    this.value = FactValue<T>.Parse(intValue);
                    break;
                case "Do":
                    double doubleValue = 0.0;
                    Double.TryParse(lastToken, out doubleValue);

                    this.value = FactValue<T>.Parse(doubleValue);
                    break;
                case "Da":
                    DateTime dateValue;
                    DateTime.TryParseExact(lastToken, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dateValue);

                    this.value = FactValue<T>.Parse(dateValue);
                    break;
                case "Url":
                    this.value = FactValue<T>.ParseURL(lastToken);
                    break;
                case "Id":
                    this.value = FactValue<T>.ParseUUID(lastToken);
                    break;
                case "Ha":
                    this.value = FactValue<T>.ParseHash(lastToken);
                    break;
                case "Q":
                    this.value = FactValue<T>.ParseDefiString(lastToken);
                    break;
                case "L":
                case "M":
                case "U":
                case "C":
                    if (this.IsBoolean(lastToken))
                    {
                        this.value = string.Equals(lastToken, "false", StringComparison.OrdinalIgnoreCase)? FactValue<T>.Parse(false) : FactValue<T>.Parse(true);
                    }
                    else
                    {
                        Regex regex = new Regex(@"^([""\“])(.*)([""\”]$)");
                        Match match = regex.Match(lastToken);

                        if (match.Success)
                        {
                            string newS = match.Groups[2].Value;
                            this.value = FactValue<T>.ParseDefiString(newS);
                        }
                        else
                        {
                            this.value = FactValue<T>.Parse(lastToken);
                        }
                    }
                    break;
            }
        }
        public void SetValue(FactValue<T> fv)
        {
            this.value = fv;
        }
        protected bool IsBoolean(string str)
        {
            return Regex.IsMatch(str,@"[FfAaLlSsEe]+") || Regex.IsMatch(str, @"[TtRrUuEe]+") ? true : false;
        }

        protected bool IsInteger(string str)
        {
            return string.Equals(str,@"No") ? true : false;
        }

        protected bool IsDouble(string str)
        {
            return string.Equals(str, @"De") ? true : false;
        }

        protected bool IsDate(string str)
        {
            return string.Equals(str, @"Da") ? true : false; 
        }

        protected bool IsURL(string str)
        {
            return string.Equals(str, @"Url") ? true : false; 
        }

        protected bool IsHash(string str)
        {
            return string.Equals(str, @"Ha") ? true : false; 
        }

        protected bool IsGUID(string str)
        {
            return string.Equals(str, @"Id") ? true : false;  
        }

    }
}