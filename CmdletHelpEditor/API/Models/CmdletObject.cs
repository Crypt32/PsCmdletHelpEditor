using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Xml.Serialization;

namespace CmdletHelpEditor.API.Models {
    [XmlInclude(typeof(ParameterDescription))]
	[XmlInclude(typeof(Example))]
	[XmlInclude(typeof(RelatedLink))]
	[XmlInclude(typeof(CommandParameterSetInfo2))]
	public class CmdletObject : INotifyPropertyChanged {
		String extraHeader, extraFooter, url, articleID;
		Boolean publish;
        SupportInfo supportInfo;
        GeneralDescription generalHelp;
        BindingList<ParameterDescription> parameters;
        ObservableCollection<Example> exampleList;
        ObservableCollection<RelatedLink> linkList;
        readonly List<String> _excludedParams = new List<String> {
                "verbose", "debug", "erroraction", "errorvariable", "outvariable", "outbuffer",
                "warningvariable", "warningaction", "pipelinevariable", "informationaction",
                "informationvariable"
            };

        public CmdletObject() {
			initialize();
		}

	    void OnBindingListChanged(object Sender, ListChangedEventArgs ChangedEventArgs) {
		    OnPropertyChanged("nested2");
	    }

	    public CmdletObject(PSObject cmdlet, Boolean cbh) : this() {
			if (cmdlet == null) { return; }
			initializeFromCmdlet(cmdlet);
			if (cbh) {
				initializeFromHelp(cmdlet);
			}
		}
		public CmdletObject(String name) : this() {
			Name = name;
			GeneralHelp = new GeneralDescription { Status = ItemStatus.Missing };
		}

        public String Name { get; set; }
		[XmlAttribute("verb")]
		public String Verb { get; set; }
		[XmlAttribute("noun")]
		public String Noun { get; set; }
        public List<String> Syntax { get; set; }
        public GeneralDescription GeneralHelp {
	        get { return generalHelp; }
            set {
                if (generalHelp != null) {
                    generalHelp.PropertyChanged -= childOnPropertyChanged;
                }
                generalHelp = value;
                if (generalHelp != null) {
                    generalHelp.PropertyChanged += childOnPropertyChanged;
                }
            }
	    }
		[XmlIgnore]
		public List<CommandParameterSetInfo> ParameterSets { get; set; }
		public List<CommandParameterSetInfo2> ParamSets { get; set; }
	    public BindingList<ParameterDescription> Parameters {
	        get { return parameters; }
			set {
				if (parameters != null) {
					parameters.ListChanged -= OnBindingListChanged;
				}
				parameters = value;
				if (parameters != null) {
					parameters.RaiseListChangedEvents = true;
					parameters.ListChanged += OnBindingListChanged;
				}
			}
	    }
	    public ObservableCollection<Example> Examples {
	        get { return exampleList; }
            set {
                if (exampleList != null) {
                    exampleList.CollectionChanged -= childOnCollectionChanged;
                }
                exampleList = value;
                if (exampleList != null) {
                    exampleList.CollectionChanged += childOnCollectionChanged;
                }
            }
	    }
	    public ObservableCollection<RelatedLink> RelatedLinks {
	        get { return linkList; }
            set {
                if (linkList != null) {
                    linkList.CollectionChanged -= childOnCollectionChanged;
                }
                linkList = value;
                if (linkList != null) {
                    linkList.CollectionChanged += childOnCollectionChanged;
                }
            }
	    }
	    public SupportInfo SupportInformation {
	        get { return supportInfo; }
            set {
                if (supportInfo != null) {
                    supportInfo.PropertyChanged -= childOnPropertyChanged;
                }
                supportInfo = value;
                if (supportInfo != null) {
                    supportInfo.PropertyChanged += childOnPropertyChanged;
                }
            }
	    }
		// advanced
		public String ExtraHeader {
			get { return extraHeader; }
			set {
				extraHeader = value;
				OnPropertyChanged("ExtraHeader");
			}
		}
		public String ExtraFooter {
			get { return extraFooter; }
			set {
				extraFooter = value;
				OnPropertyChanged("ExtraFooter");
			}
		}
		// rest
		public Boolean Publish {
			get { return publish; }
			set {
				publish = value;
				OnPropertyChanged("Publish");
			}
		}
		public String URL {
			get { return url; }
			set {
				url = value;
				OnPropertyChanged("URL");
			}
		}
		public String ArticleIDString {
			get { return articleID; }
			set {
				articleID = value;
				OnPropertyChanged("ArticleIDString");
			}
		}
		void initialize() {
			ParamSets = new List<CommandParameterSetInfo2>();
			Parameters = new BindingList<ParameterDescription>();
			Examples = new ObservableCollection<Example>();
			RelatedLinks = new ObservableCollection<RelatedLink>();
		}

        #region nested object event handlers
        void childOnCollectionChanged(Object sender, NotifyCollectionChangedEventArgs e) {
	        switch (e.Action) {
                case NotifyCollectionChangedAction.Add:
                    ((INotifyPropertyChanged)e.NewItems[0]).PropertyChanged += childOnPropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Remove:
                    ((INotifyPropertyChanged)e.OldItems[0]).PropertyChanged -= childOnPropertyChanged;
                    break;
                case NotifyCollectionChangedAction.Reset:
                    if (e.OldItems == null) { break; }
	                foreach (INotifyPropertyChanged itemToRemove in e.OldItems) {
                        itemToRemove.PropertyChanged -= childOnPropertyChanged;
	                }
                    break;
	        }
	    }
	    void childOnPropertyChanged(Object sender, PropertyChangedEventArgs e) {
	        OnPropertyChanged("nested");
	    }
        #endregion

        void initializeFromCmdlet(PSObject cmdlet) {
			Name = (String)cmdlet.Members["Name"].Value;
			Verb = (String)cmdlet.Members["Verb"].Value;
			Noun = (String)cmdlet.Members["Noun"].Value;
			GeneralHelp = new GeneralDescription {Status = ItemStatus.New};
            Parameters = new BindingList<ParameterDescription>();
            ParamSets = new List<CommandParameterSetInfo2>();
			getParameterSets(cmdlet);
			getParameters();
            getOutputtypes(cmdlet);
            getSyntax(cmdlet);
			RelatedLinks = new ObservableCollection<RelatedLink>();
			Examples = new ObservableCollection<Example>();
			SupportInformation = new SupportInfo();
		}
		void initializeFromHelp(PSObject cmdlet) {
			PSObject help = (PSObject) cmdlet.Members["syn"].Value;
			if (help.Members["Description"].Value == null) { return; }
			if (String.IsNullOrEmpty((String)help.Members["Synopsis"].Value)) { return; }
			// synopsis
			GeneralHelp.Synopsis = (String)help.Members["Synopsis"].Value;
			// description
			String description = ((PSObject[]) help.Members["Description"].Value)
				.Aggregate(String.Empty, (current, paragraph) => current + (paragraph.Members["Text"].Value + Environment.NewLine));
			GeneralHelp.Description = description.TrimEnd();
			// notes
			//try {
				if (help.Properties["alertSet"] != null) {
					importNotesHelp((PSObject)help.Members["alertSet"].Value);
				}
			//} catch { }
			// input type
				if (help.Properties["inputTypes"] != null) {
					importTypes((PSObject)help.Members["inputTypes"].Value, true);
				}
			// return type
				if (help.Properties["returnValues"] != null) {
					importTypes((PSObject)help.Members["returnValues"].Value, false);
				}
			// parameters
			try {
				PSObject param = (PSObject)help.Members["parameters"].Value;
				importParamHelp(param);
			} catch { }
			// examples
			try {
				PSObject examples = (PSObject)help.Members["examples"].Value;
				importExamples(examples);
			} catch { }
			// related links
			try {
				PSObject relinks = (PSObject)help.Members["relatedLinks"].Value;
				importRelinks(relinks);
			} catch { }
		}
		void importNotesHelp(PSObject note) {
			GeneralHelp.Notes = (String)((PSObject[])note.Members["alert"].Value)[0].Members["Text"].Value;
		}
		void importTypes(PSObject types, Boolean input) {
			List<PSObject> iType = new List<PSObject>();
			if (input) {
				PSObject value = types.Members["inputType"].Value as PSObject;
				if (value != null) {
					iType.Add(value);
				} else {
					iType = new List<PSObject>((PSObject[])types.Members["inputType"].Value);
				}
			} else {
				PSObject value = types.Members["returnValue"].Value as PSObject;
				if (value != null) {
					iType.Add(value);
				} else {
					iType = new List<PSObject>((PSObject[])types.Members["returnValue"].Value);
				}
			}
			
			List<String> links = new List<String>();
			List<String> urls = new List<String>();
			List<String> descs = new List<String>();

			foreach (PSObject type in iType) {
				PSObject internalType = (PSObject) type.Members["type"].Value;
				if (internalType.Properties["name"] == null) {
					links.Add(String.Empty);
				} else {
					PSObject name = (PSObject)internalType.Members["name"].Value;
					links.Add((String)name.BaseObject);
				}
				if (internalType.Properties["uri"] == null) {
					urls.Add(String.Empty);
				} else {
					PSObject uri = (PSObject)internalType.Members["uri"].Value;
					urls.Add((String)uri.BaseObject);
				}

				if (type.Properties["description"] == null) { continue; }
				PSObject[] descriptionBase = type.Members["description"].Value as PSObject[];
				if (descriptionBase != null) {
					String description = (descriptionBase)
						.Aggregate(String.Empty, (current, paragraph) => current + (paragraph.Members["Text"].Value + Environment.NewLine));
					descs.Add(String.IsNullOrEmpty(description)
						? String.Empty
						: description.TrimEnd());
				}
			}
			if (input) {
				GeneralHelp.InputType = String.Join(";", links);
				GeneralHelp.InputUrl = String.Join(";", urls);
				GeneralHelp.InputTypeDescription = String.Join(";", descs);
			} else {
				GeneralHelp.ReturnType = String.Join(";", links);
				GeneralHelp.ReturnUrl = String.Join(";", urls);
				GeneralHelp.ReturnTypeDescription = String.Join(";", descs);
			}
		}
		void importParamHelp(PSObject helpParameters) {
			List<PSObject> paras = new List<PSObject>();
			if (!(helpParameters.Members["parameter"].Value is PSObject)) {
				paras = new List<PSObject>((PSObject[])helpParameters.Members["parameter"].Value);
			} else {
				paras.Add((PSObject)helpParameters.Members["parameter"].Value);
			}
			foreach (PSObject param in paras) {
				String name = (String)((PSObject)param.Members["name"].Value).BaseObject;
				String description = ((PSObject[]) param.Members["Description"].Value)
				.Aggregate(String.Empty, (current, paragraph) => current + (paragraph.Members["Text"].Value + Environment.NewLine));
				String defaultValue = (String)((PSObject)param.Members["defaultValue"].Value).BaseObject;
				ParameterDescription currentParam = Parameters.Single(x => x.Name == name);
				currentParam.Description = description;
				currentParam.DefaultValue = defaultValue;
			}
		}
		void importExamples(PSObject example) {
			List<PSObject> examples = new List<PSObject>();
			if (!(example.Members["example"].Value is PSObject)) {
				examples = new List<PSObject>((PSObject[])example.Members["example"].Value);
			} else {
				examples.Add((PSObject)example.Members["example"].Value);
			}
			foreach (PSObject ex in examples) {
				String title = ((String)((PSObject)ex.Members["title"].Value).BaseObject).Replace("-", String.Empty).Trim();
				String code = (String)((PSObject)ex.Members["code"].Value).BaseObject;
				String description = ((PSObject[])ex.Members["remarks"].Value)
				.Aggregate(String.Empty, (current, paragraph) => current + (paragraph.Members["Text"].Value + Environment.NewLine));
				Examples.Add(new Example {
					Name = title,
					Cmd = code,
					Description = description
				});
			}
		}
		void importRelinks(PSObject relink) {
			List<PSObject> relinks = new List<PSObject>();
			if (!(relink.Members["navigationLink"].Value is PSObject)) {
				relinks = new List<PSObject>((PSObject[])relink.Members["navigationLink"].Value);
			} else {
				relinks.Add((PSObject)relink.Members["navigationLink"].Value);
			}
			foreach (PSObject link in relinks) {
				String linkText = ((String)((PSObject)link.Members["linkText"].Value).BaseObject).Replace("-", String.Empty).Trim();
				String uri = (String)((PSObject)link.Members["uri"].Value).BaseObject;
				RelatedLinks.Add(new RelatedLink {
					LinkText = linkText,
					LinkUrl = uri
				});
			}
		}
		void getParameterSets(PSObject cmdlet) {
			ParameterSets = new List<CommandParameterSetInfo>();
			if (cmdlet.Members["ParameterSets"].Value != null) {
				ParameterSets = new List<CommandParameterSetInfo>((ReadOnlyCollection<CommandParameterSetInfo>)cmdlet.Members["ParameterSets"].Value);
				foreach (CommandParameterSetInfo paramInfo in ParameterSets) {
					CommandParameterSetInfo2 info = new CommandParameterSetInfo2 { Name = paramInfo.Name };
					foreach (var param in paramInfo.Parameters) {
						info.Parameters.Add(param.Name);
					}
					ParamSets.Add(info);
				}
			}
		}
		void getParameters() {
			if (ParameterSets.Count == 0) { return; }
			foreach (CommandParameterSetInfo paramset in ParameterSets) {
				if (paramset.Parameters.Count == 0) { return; }
				foreach (CommandParameterInfo param in from param in paramset.Parameters where !_excludedParams.Contains(param.Name.ToLower()) let para = new ParameterDescription(param) where !Parameters.Contains(para) select param) {
					Parameters.Add(new ParameterDescription(param));
				}
			}
		}
        void getOutputtypes(PSObject cmdlet) {
            var outputTypeMember = cmdlet.Members["OutputType"];
            if (outputTypeMember == null) {
                return;
            }
            var outputTypeNames = outputTypeMember.Value as IEnumerable<PSTypeName>;
            if (outputTypeNames == null) {
                return;
            }
            string joined = String.Join(";", outputTypeNames.Select(tn => tn.Name));
            GeneralHelp.ReturnType = joined;
        }
        void getSyntax(PSObject cmdlet) {
			Syntax = new List<String>();
			foreach (CommandParameterSetInfo paraset in ParameterSets) {
				String syntaxItem = Convert.ToString(cmdlet.Members["name"].Value);
				foreach (CommandParameterInfo item in paraset.Parameters) {
					if (_excludedParams.Contains(item.Name.ToLower())) { continue; }
					Boolean named = item.Position < 0;
					// fetch param type
					String paramType = String.Empty;
					CommandParameterInfo item1 = item;
					foreach (ParameterDescription param in Parameters.Where(param => item1.Name == param.Name)) {
						paramType = param.Type;
					}
					// fetch VelidateSet attribute
					String validateSet = String.Empty;
					foreach (Attribute attribute in item.Attributes) {
						Boolean found = false;
						validateSet = String.Empty;
						switch (attribute.TypeId.ToString()) {
							case "System.Management.Automation.ValidateSetAttribute":
								validateSet += " {";
								validateSet += String.Join(" | ", ((ValidateSetAttribute)attribute).ValidValues);
								validateSet += "} ";
								found = true;
								break;
						}
						if (found) { break; }
					}
					if (item.IsMandatory && named) {
						syntaxItem += " -" + item.Name + " <" + paramType + ">" + validateSet;
					} else if (item.IsMandatory) {
						syntaxItem += " [-" + item.Name + "] <" + paramType + ">" + validateSet;
					} else if (!named) {
						syntaxItem += " [[-" + item.Name + "] <" + paramType + ">" + validateSet + "]";
					} else if (!String.IsNullOrEmpty(paramType) && paramType != "SwitchParameter") {
						syntaxItem += " [-" + item.Name + " <" + paramType + ">" + validateSet + "]";
					} else {
						syntaxItem += " [-" + item.Name + validateSet + "]";
					}
				}
				Syntax.Add(syntaxItem);
			}
		}

		public void UpdateParamSets() {
			if (ParameterSets == null) { return; }
			ParamSets.Clear();
			foreach (CommandParameterSetInfo paramInfo in ParameterSets) {
				CommandParameterSetInfo2 info = new CommandParameterSetInfo2 { Name = paramInfo.Name };
				foreach (var param in paramInfo.Parameters) {
					info.Parameters.Add(param.Name);
				}
				ParamSets.Add(info);
			}
		}
		public override String ToString() {
			return Name;
		}
		public override Boolean Equals(object obj) {
		    if (ReferenceEquals(null, obj)) return false;
		    if (ReferenceEquals(this, obj)) return true;
		    if (obj.GetType() != this.GetType()) return false;
		    return Equals((CmdletObject) obj);
		}

        protected bool Equals(CmdletObject other) {
            return string.Equals(Name, other.Name);
        }

        public override int GetHashCode() {
            return Name != null ? Name.GetHashCode() : 0;
        }

        void OnPropertyChanged(String name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
	}
}