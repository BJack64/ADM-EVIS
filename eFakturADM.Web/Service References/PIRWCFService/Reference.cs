﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace eFakturADM.Web.PIRWCFService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EmployeeInformation", Namespace="http://schemas.datacontract.org/2004/07/PIRService")]
    [System.SerializableAttribute()]
    public partial class EmployeeInformation : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string BirthDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DeptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DivField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string EndDateField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string FirstNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GGI1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string GGI2Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string LastNameField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PhoneNumberField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PhotoIDField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string PositionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ServiceField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SiteField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StartDateField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string BirthDate {
            get {
                return this.BirthDateField;
            }
            set {
                if ((object.ReferenceEquals(this.BirthDateField, value) != true)) {
                    this.BirthDateField = value;
                    this.RaisePropertyChanged("BirthDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Dept {
            get {
                return this.DeptField;
            }
            set {
                if ((object.ReferenceEquals(this.DeptField, value) != true)) {
                    this.DeptField = value;
                    this.RaisePropertyChanged("Dept");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Div {
            get {
                return this.DivField;
            }
            set {
                if ((object.ReferenceEquals(this.DivField, value) != true)) {
                    this.DivField = value;
                    this.RaisePropertyChanged("Div");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string EndDate {
            get {
                return this.EndDateField;
            }
            set {
                if ((object.ReferenceEquals(this.EndDateField, value) != true)) {
                    this.EndDateField = value;
                    this.RaisePropertyChanged("EndDate");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FirstName {
            get {
                return this.FirstNameField;
            }
            set {
                if ((object.ReferenceEquals(this.FirstNameField, value) != true)) {
                    this.FirstNameField = value;
                    this.RaisePropertyChanged("FirstName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GGI1 {
            get {
                return this.GGI1Field;
            }
            set {
                if ((object.ReferenceEquals(this.GGI1Field, value) != true)) {
                    this.GGI1Field = value;
                    this.RaisePropertyChanged("GGI1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string GGI2 {
            get {
                return this.GGI2Field;
            }
            set {
                if ((object.ReferenceEquals(this.GGI2Field, value) != true)) {
                    this.GGI2Field = value;
                    this.RaisePropertyChanged("GGI2");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string LastName {
            get {
                return this.LastNameField;
            }
            set {
                if ((object.ReferenceEquals(this.LastNameField, value) != true)) {
                    this.LastNameField = value;
                    this.RaisePropertyChanged("LastName");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PhoneNumber {
            get {
                return this.PhoneNumberField;
            }
            set {
                if ((object.ReferenceEquals(this.PhoneNumberField, value) != true)) {
                    this.PhoneNumberField = value;
                    this.RaisePropertyChanged("PhoneNumber");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string PhotoID {
            get {
                return this.PhotoIDField;
            }
            set {
                if ((object.ReferenceEquals(this.PhotoIDField, value) != true)) {
                    this.PhotoIDField = value;
                    this.RaisePropertyChanged("PhotoID");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Position {
            get {
                return this.PositionField;
            }
            set {
                if ((object.ReferenceEquals(this.PositionField, value) != true)) {
                    this.PositionField = value;
                    this.RaisePropertyChanged("Position");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Service {
            get {
                return this.ServiceField;
            }
            set {
                if ((object.ReferenceEquals(this.ServiceField, value) != true)) {
                    this.ServiceField = value;
                    this.RaisePropertyChanged("Service");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Site {
            get {
                return this.SiteField;
            }
            set {
                if ((object.ReferenceEquals(this.SiteField, value) != true)) {
                    this.SiteField = value;
                    this.RaisePropertyChanged("Site");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StartDate {
            get {
                return this.StartDateField;
            }
            set {
                if ((object.ReferenceEquals(this.StartDateField, value) != true)) {
                    this.StartDateField = value;
                    this.RaisePropertyChanged("StartDate");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Organization", Namespace="http://schemas.datacontract.org/2004/07/PIRService")]
    [System.SerializableAttribute()]
    public partial class Organization : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DeptField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DivField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ServField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Dept {
            get {
                return this.DeptField;
            }
            set {
                if ((object.ReferenceEquals(this.DeptField, value) != true)) {
                    this.DeptField = value;
                    this.RaisePropertyChanged("Dept");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Div {
            get {
                return this.DivField;
            }
            set {
                if ((object.ReferenceEquals(this.DivField, value) != true)) {
                    this.DivField = value;
                    this.RaisePropertyChanged("Div");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Serv {
            get {
                return this.ServField;
            }
            set {
                if ((object.ReferenceEquals(this.ServField, value) != true)) {
                    this.ServField = value;
                    this.RaisePropertyChanged("Serv");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PIRWCFService.IPIRWCFService")]
    public interface IPIRWCFService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPIRWCFService/GetDataByGGI", ReplyAction="http://tempuri.org/IPIRWCFService/GetDataByGGIResponse")]
        eFakturADM.Web.PIRWCFService.EmployeeInformation GetDataByGGI(string GGI);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPIRWCFService/GetDataByField", ReplyAction="http://tempuri.org/IPIRWCFService/GetDataByFieldResponse")]
        eFakturADM.Web.PIRWCFService.EmployeeInformation[] GetDataByField(string where, string orderBy);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPIRWCFService/GetAllEntity", ReplyAction="http://tempuri.org/IPIRWCFService/GetAllEntityResponse")]
        eFakturADM.Web.PIRWCFService.Organization[] GetAllEntity();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPIRWCFServiceChannel : global::eFakturADM.Web.PIRWCFService.IPIRWCFService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PIRWCFServiceClient : System.ServiceModel.ClientBase<global::eFakturADM.Web.PIRWCFService.IPIRWCFService>, global::eFakturADM.Web.PIRWCFService.IPIRWCFService {
        
        public PIRWCFServiceClient() {
        }
        
        public PIRWCFServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PIRWCFServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PIRWCFServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PIRWCFServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public eFakturADM.Web.PIRWCFService.EmployeeInformation GetDataByGGI(string GGI) {
            return base.Channel.GetDataByGGI(GGI);
        }
        
        public eFakturADM.Web.PIRWCFService.EmployeeInformation[] GetDataByField(string where, string orderBy) {
            return base.Channel.GetDataByField(where, orderBy);
        }
        
        public eFakturADM.Web.PIRWCFService.Organization[] GetAllEntity() {
            return base.Channel.GetAllEntity();
        }
    }
}
