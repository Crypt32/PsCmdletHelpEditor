using System;
using System.Collections.Generic;

namespace PsCmdletHelpEditor.BLL.Tools {
    static class XmlTokenizer {
        /// <summary>
        /// Iterate over characters one by one to tokenize the Xml String.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static IEnumerable<XmlToken> LoopTokenize(String str) {
            //  Temp variables to build up the current token
            List<Char> currentTokenText = new List<Char>();
            //  Represents the list of tokens to be returned
            List<XmlToken> tokens = new List<XmlToken>();
            //  Represents the index of the first character in the token
            Int32 tokenIndex = 0;
            Boolean isStartTag = false;
            Boolean isComment = false;
            Boolean isQuote = false;
            Boolean isAttribute = false;

            for (Int32 index = 0; index < str.Length; index++) {
                //  Get the current character
                Char c = str[index];
                //  Skip the "ZERO WIDTH NO-BREAK SPACE" character resulting from encoding
                if (c == 65279) { continue; }

                //  Handle the escape sequence case
                if (c == '&') {
                    if (currentTokenText.Count > 0) {
                        XmlToken token = new XmlToken { Index = tokenIndex, Text = new String(currentTokenText.ToArray()) };
                        tokens.Add(token);
                        currentTokenText.Clear();
                        //  Determine token type
                        if (isQuote) {
                            token.Type = XmlTokenEnum.Value;
                        } else if (isComment) {
                            token.Type = XmlTokenEnum.Comment;
                        } else if (isStartTag) {
                            token.Type = isAttribute ? XmlTokenEnum.Attribute : XmlTokenEnum.Element;
                        } else {
                            token.Type = XmlTokenEnum.None;
                        }
                    }
                    currentTokenText.Add('&');
                    XmlToken escapeToken = new XmlToken { Type = XmlTokenEnum.Escape, Index = index };
                    index++;
                    while (index < str.Length && Char.IsLetterOrDigit(str[index])) {
                        currentTokenText.Add(str[index]);
                        index++;
                    }
                    if (index < str.Length && c == ';') {
                        currentTokenText.Add(';');
                        index++;
                    }
                    escapeToken.Text = new String(currentTokenText.ToArray());
                    escapeToken.Text += ";";
                    currentTokenText.Clear();
                    tokens.Add(escapeToken);
                    continue;
                }

                //  Only if the character is not between "" that is in a tag
                if (!isQuote) {
                    //  Only if the character is not in a comment
                    if (!isComment) {
                        //  Only if we already have a start tag
                        if (isStartTag) {
                            if (Char.IsLetterOrDigit(c)) {
                                //  We're starting to build up a token, so save its index
                                if (currentTokenText.Count == 0) {
                                    tokenIndex = index;
                                }

                                currentTokenText.Add(c);
                            } else {
                                //  Add the previous token that could be an element or an attribute
                                if (currentTokenText.Count > 0) {
                                    XmlToken token = new XmlToken { Text = new String(currentTokenText.ToArray()) };
                                    currentTokenText.Clear();
                                    token.Index = tokenIndex;

                                    token.Type = isAttribute ? XmlTokenEnum.Attribute : XmlTokenEnum.Element;

                                    tokens.Add(token);
                                }

                                //  Check if we have something like <!-- to flag that we have a comment
                                switch (c) {
                                    case '-':
                                        if (index - 2 >= 0 && index + 1 < str.Length &&
                                            (str[index - 2] == '<' && str[index - 1] == '!' && str[index + 1] == '-')) {
                                            isStartTag = false;
                                            isComment = true;
                                            tokens[tokens.Count - 1].Type = tokens[tokens.Count - 2].Type = XmlTokenEnum.Comment;
                                            //index += 1;
                                        }
                                        break;
                                    case '>':
                                        isStartTag = false;
                                        isAttribute = false;
                                        break;
                                    case '<':
                                        isAttribute = false;
                                        break;
                                    case '"':
                                        isQuote = true;
                                        break;
                                }

                                //  Check if we now have an attribute
                                if (Char.IsWhiteSpace(c)) {
                                    isAttribute = true;
                                    tokens.Add(new XmlToken(Convert.ToString(c), index, XmlTokenEnum.None));
                                } else {
                                    tokens.Add(new XmlToken(Convert.ToString(c), index, XmlTokenEnum.SpecialChar));
                                }
                            }
                        } else {
                            //  If we didn't have a start tag, check if we now have one
                            if (c == '<') {
                                if (currentTokenText.Count > 0) {
                                    XmlToken token = new XmlToken {
                                        Index = tokenIndex,
                                        Text = new String(currentTokenText.ToArray()),
                                        Type = XmlTokenEnum.None
                                    };
                                    tokens.Add(token);
                                    currentTokenText.Clear();
                                }
                                isStartTag = true;
                                tokens.Add(new XmlToken("<", index, XmlTokenEnum.SpecialChar));
                            } else {
                                if (currentTokenText.Count == 0) { tokenIndex = index; }
                                currentTokenText.Add(c);
                            }
                        }
                    } else {
                        //  In case we have a comment
                        //  We're starting to build up a token, so save its index
                        if (currentTokenText.Count == 0) { tokenIndex = index; }
                        currentTokenText.Add(c);
                        //  Check if we have something like --> to see if we're closing a comment
                        //  or if we're at the end
                        if (index + 2 < str.Length) {
                            if (c == '-') {
                                if (str[index + 1] == '-' && str[index + 2] == '>') {
                                    isComment = false;
                                    index += 2;
                                }
                            }
                        } else { isComment = false; }
                        if (!isComment) {
                            XmlToken token = new XmlToken {
                                Type = XmlTokenEnum.Comment,
                                Index = tokenIndex,
                                Text = new String(currentTokenText.ToArray())
                            };
                            token.Text += "->";
                            tokens.Add(token);
                            currentTokenText.Clear();
                        }
                    }
                } else {
                    //  We're starting to build up a token, so save its index
                    if (currentTokenText.Count == 0) { tokenIndex = index; }
                    //  Check if we no longer have a quote
                    if (c == '"') {
                        isQuote = false;
                        currentTokenText.Add('\"');
                        XmlToken token = new XmlToken {
                            Type = XmlTokenEnum.Value,
                            Index = tokenIndex,
                            Text = new String(currentTokenText.ToArray())
                        };
                        tokens.Add(token);
                        currentTokenText.Clear();
                    } else { currentTokenText.Add(c); }
                }
            }
            //  Handle the last element
            if (currentTokenText.Count > 0) {
                XmlToken token = new XmlToken { Index = tokenIndex, Text = new String(currentTokenText.ToArray()) };
                tokens.Add(token);
                currentTokenText.Clear();
                //  Determine token type
                if (isQuote) {
                    token.Type = XmlTokenEnum.Value;
                } else if (isComment) {
                    token.Type = XmlTokenEnum.Comment;
                } else if (isStartTag) {
                    token.Type = isAttribute ? XmlTokenEnum.Attribute : XmlTokenEnum.Element;
                } else {
                    token.Type = XmlTokenEnum.None;
                }
            }
            return tokens;
        }
    }
}
