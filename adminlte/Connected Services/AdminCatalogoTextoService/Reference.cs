﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace adminlte.AdminCatalogoTextoService {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="AdminCatalogoTextoSet", Namespace="http://schemas.datacontract.org/2004/07/WebObjetos")]
    [System.SerializableAttribute()]
    public partial class AdminCatalogoTextoSet : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> ltAdminCatalogoTextoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> ltAdminCatalogoTextoEliminadoField;
        
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
        public System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> ltAdminCatalogoTexto {
            get {
                return this.ltAdminCatalogoTextoField;
            }
            set {
                if ((object.ReferenceEquals(this.ltAdminCatalogoTextoField, value) != true)) {
                    this.ltAdminCatalogoTextoField = value;
                    this.RaisePropertyChanged("ltAdminCatalogoTexto");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> ltAdminCatalogoTextoEliminado {
            get {
                return this.ltAdminCatalogoTextoEliminadoField;
            }
            set {
                if ((object.ReferenceEquals(this.ltAdminCatalogoTextoEliminadoField, value) != true)) {
                    this.ltAdminCatalogoTextoEliminadoField = value;
                    this.RaisePropertyChanged("ltAdminCatalogoTextoEliminado");
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
    [System.Runtime.Serialization.DataContractAttribute(Name="AdminCatalogoTextoEntity", Namespace="http://schemas.datacontract.org/2004/07/WebObjetos")]
    [System.SerializableAttribute()]
    public partial class AdminCatalogoTextoEntity : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal Decimal1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal Decimal2Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string DescripcionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long Entero1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private long Entero2Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime Fecha1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private System.DateTime Fecha2Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool Logico1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool Logico2Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private decimal MontoSugeridoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string ObservacionField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string SubCompaniaField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string TextoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string Texto0Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string Texto1Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string Texto2Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string Texto3Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string Texto4Field;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UClaseField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private adminlte.AdminCatalogoTextoService.Estado UEstadoField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool USelField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string UUsuarioField;
        
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
        public decimal Decimal1 {
            get {
                return this.Decimal1Field;
            }
            set {
                if ((this.Decimal1Field.Equals(value) != true)) {
                    this.Decimal1Field = value;
                    this.RaisePropertyChanged("Decimal1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Decimal2 {
            get {
                return this.Decimal2Field;
            }
            set {
                if ((this.Decimal2Field.Equals(value) != true)) {
                    this.Decimal2Field = value;
                    this.RaisePropertyChanged("Decimal2");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Descripcion {
            get {
                return this.DescripcionField;
            }
            set {
                if ((object.ReferenceEquals(this.DescripcionField, value) != true)) {
                    this.DescripcionField = value;
                    this.RaisePropertyChanged("Descripcion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Entero1 {
            get {
                return this.Entero1Field;
            }
            set {
                if ((this.Entero1Field.Equals(value) != true)) {
                    this.Entero1Field = value;
                    this.RaisePropertyChanged("Entero1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long Entero2 {
            get {
                return this.Entero2Field;
            }
            set {
                if ((this.Entero2Field.Equals(value) != true)) {
                    this.Entero2Field = value;
                    this.RaisePropertyChanged("Entero2");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Fecha1 {
            get {
                return this.Fecha1Field;
            }
            set {
                if ((this.Fecha1Field.Equals(value) != true)) {
                    this.Fecha1Field = value;
                    this.RaisePropertyChanged("Fecha1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Fecha2 {
            get {
                return this.Fecha2Field;
            }
            set {
                if ((this.Fecha2Field.Equals(value) != true)) {
                    this.Fecha2Field = value;
                    this.RaisePropertyChanged("Fecha2");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Logico1 {
            get {
                return this.Logico1Field;
            }
            set {
                if ((this.Logico1Field.Equals(value) != true)) {
                    this.Logico1Field = value;
                    this.RaisePropertyChanged("Logico1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool Logico2 {
            get {
                return this.Logico2Field;
            }
            set {
                if ((this.Logico2Field.Equals(value) != true)) {
                    this.Logico2Field = value;
                    this.RaisePropertyChanged("Logico2");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal MontoSugerido {
            get {
                return this.MontoSugeridoField;
            }
            set {
                if ((this.MontoSugeridoField.Equals(value) != true)) {
                    this.MontoSugeridoField = value;
                    this.RaisePropertyChanged("MontoSugerido");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Observacion {
            get {
                return this.ObservacionField;
            }
            set {
                if ((object.ReferenceEquals(this.ObservacionField, value) != true)) {
                    this.ObservacionField = value;
                    this.RaisePropertyChanged("Observacion");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string SubCompania {
            get {
                return this.SubCompaniaField;
            }
            set {
                if ((object.ReferenceEquals(this.SubCompaniaField, value) != true)) {
                    this.SubCompaniaField = value;
                    this.RaisePropertyChanged("SubCompania");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Texto {
            get {
                return this.TextoField;
            }
            set {
                if ((object.ReferenceEquals(this.TextoField, value) != true)) {
                    this.TextoField = value;
                    this.RaisePropertyChanged("Texto");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Texto0 {
            get {
                return this.Texto0Field;
            }
            set {
                if ((object.ReferenceEquals(this.Texto0Field, value) != true)) {
                    this.Texto0Field = value;
                    this.RaisePropertyChanged("Texto0");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Texto1 {
            get {
                return this.Texto1Field;
            }
            set {
                if ((object.ReferenceEquals(this.Texto1Field, value) != true)) {
                    this.Texto1Field = value;
                    this.RaisePropertyChanged("Texto1");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Texto2 {
            get {
                return this.Texto2Field;
            }
            set {
                if ((object.ReferenceEquals(this.Texto2Field, value) != true)) {
                    this.Texto2Field = value;
                    this.RaisePropertyChanged("Texto2");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Texto3 {
            get {
                return this.Texto3Field;
            }
            set {
                if ((object.ReferenceEquals(this.Texto3Field, value) != true)) {
                    this.Texto3Field = value;
                    this.RaisePropertyChanged("Texto3");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Texto4 {
            get {
                return this.Texto4Field;
            }
            set {
                if ((object.ReferenceEquals(this.Texto4Field, value) != true)) {
                    this.Texto4Field = value;
                    this.RaisePropertyChanged("Texto4");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UClase {
            get {
                return this.UClaseField;
            }
            set {
                if ((object.ReferenceEquals(this.UClaseField, value) != true)) {
                    this.UClaseField = value;
                    this.RaisePropertyChanged("UClase");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public adminlte.AdminCatalogoTextoService.Estado UEstado {
            get {
                return this.UEstadoField;
            }
            set {
                if ((this.UEstadoField.Equals(value) != true)) {
                    this.UEstadoField = value;
                    this.RaisePropertyChanged("UEstado");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool USel {
            get {
                return this.USelField;
            }
            set {
                if ((this.USelField.Equals(value) != true)) {
                    this.USelField = value;
                    this.RaisePropertyChanged("USel");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string UUsuario {
            get {
                return this.UUsuarioField;
            }
            set {
                if ((object.ReferenceEquals(this.UUsuarioField, value) != true)) {
                    this.UUsuarioField = value;
                    this.RaisePropertyChanged("UUsuario");
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Estado", Namespace="http://schemas.datacontract.org/2004/07/WebObjetos")]
    public enum Estado : int {
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Added = 0,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Unchanged = 1,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Modified = 2,
        
        [System.Runtime.Serialization.EnumMemberAttribute()]
        Deleted = 3,
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AdminCatalogoTextoService.AdminCatalogoTextoInterface")]
    public interface AdminCatalogoTextoInterface {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebNuevo", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebNuevoResponse")]
        adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet WebNuevo(string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebNuevo", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebNuevoResponse")]
        System.Threading.Tasks.Task<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet> WebNuevoAsync(string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebGuardar", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebGuardarResponse")]
        long WebGuardar(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, bool EsNuevo, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebGuardar", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebGuardarResponse")]
        System.Threading.Tasks.Task<long> WebGuardarAsync(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, bool EsNuevo, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebEliminar", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebEliminarResponse")]
        long WebEliminar(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebEliminar", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebEliminarResponse")]
        System.Threading.Tasks.Task<long> WebEliminarAsync(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebEliminarDetalle", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebEliminarDetalleResponse")]
        long WebEliminarDetalle(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebEliminarDetalle", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebEliminarDetalleResponse")]
        System.Threading.Tasks.Task<long> WebEliminarDetalleAsync(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebSeleccionar", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebSeleccionarResponse")]
        adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet WebSeleccionar(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebSeleccionar", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebSeleccionarResponse")]
        System.Threading.Tasks.Task<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet> WebSeleccionarAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebRecalcular", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebRecalcularResponse")]
        adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet WebRecalcular(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebRecalcular", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebRecalcularResponse")]
        System.Threading.Tasks.Task<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet> WebRecalcularAsync(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoBloquear", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoBloquearRespo" +
            "nse")]
        long WebAdminCatalogoTextoBloquear(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoBloquear", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoBloquearRespo" +
            "nse")]
        System.Threading.Tasks.Task<long> WebAdminCatalogoTextoBloquearAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoDesbloquear", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoDesbloquearRe" +
            "sponse")]
        long WebAdminCatalogoTextoDesbloquear(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoDesbloquear", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoDesbloquearRe" +
            "sponse")]
        System.Threading.Tasks.Task<long> WebAdminCatalogoTextoDesbloquearAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoEsBloqueadoPo" +
            "r", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoEsBloqueadoPo" +
            "rResponse")]
        string WebAdminCatalogoTextoEsBloqueadoPor(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoEsBloqueadoPo" +
            "r", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoEsBloqueadoPo" +
            "rResponse")]
        System.Threading.Tasks.Task<string> WebAdminCatalogoTextoEsBloqueadoPorAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoEsBloqueado", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoEsBloqueadoRe" +
            "sponse")]
        bool WebAdminCatalogoTextoEsBloqueado(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoEsBloqueado", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoEsBloqueadoRe" +
            "sponse")]
        System.Threading.Tasks.Task<bool> WebAdminCatalogoTextoEsBloqueadoAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarTo" +
            "do", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarTo" +
            "doResponse")]
        System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> WebAdminCatalogoTextoSeleccionarTodo(string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarTo" +
            "do", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarTo" +
            "doResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity>> WebAdminCatalogoTextoSeleccionarTodoAsync(string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionar", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarRe" +
            "sponse")]
        System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> WebAdminCatalogoTextoSeleccionar(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionar", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarRe" +
            "sponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity>> WebAdminCatalogoTextoSeleccionarAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarXS" +
            "ubCompania", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarXS" +
            "ubCompaniaResponse")]
        System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> WebAdminCatalogoTextoSeleccionarXSubCompania(string SubCompania, string strAKASesion, string strAKASubCompania);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarXS" +
            "ubCompania", ReplyAction="http://tempuri.org/AdminCatalogoTextoInterface/WebAdminCatalogoTextoSeleccionarXS" +
            "ubCompaniaResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity>> WebAdminCatalogoTextoSeleccionarXSubCompaniaAsync(string SubCompania, string strAKASesion, string strAKASubCompania);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface AdminCatalogoTextoInterfaceChannel : adminlte.AdminCatalogoTextoService.AdminCatalogoTextoInterface, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AdminCatalogoTextoInterfaceClient : System.ServiceModel.ClientBase<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoInterface>, adminlte.AdminCatalogoTextoService.AdminCatalogoTextoInterface {
        
        public AdminCatalogoTextoInterfaceClient() {
        }
        
        public AdminCatalogoTextoInterfaceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public AdminCatalogoTextoInterfaceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AdminCatalogoTextoInterfaceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public AdminCatalogoTextoInterfaceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet WebNuevo(string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebNuevo(strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet> WebNuevoAsync(string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebNuevoAsync(strAKASesion, strAKASubCompania);
        }
        
        public long WebGuardar(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, bool EsNuevo, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebGuardar(setAdminCatalogoTexto, EsNuevo, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<long> WebGuardarAsync(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, bool EsNuevo, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebGuardarAsync(setAdminCatalogoTexto, EsNuevo, strAKASesion, strAKASubCompania);
        }
        
        public long WebEliminar(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebEliminar(setAdminCatalogoTexto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<long> WebEliminarAsync(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebEliminarAsync(setAdminCatalogoTexto, strAKASesion, strAKASubCompania);
        }
        
        public long WebEliminarDetalle(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebEliminarDetalle(setAdminCatalogoTexto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<long> WebEliminarDetalleAsync(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebEliminarDetalleAsync(setAdminCatalogoTexto, strAKASesion, strAKASubCompania);
        }
        
        public adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet WebSeleccionar(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebSeleccionar(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet> WebSeleccionarAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebSeleccionarAsync(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet WebRecalcular(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebRecalcular(setAdminCatalogoTexto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet> WebRecalcularAsync(adminlte.AdminCatalogoTextoService.AdminCatalogoTextoSet setAdminCatalogoTexto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebRecalcularAsync(setAdminCatalogoTexto, strAKASesion, strAKASubCompania);
        }
        
        public long WebAdminCatalogoTextoBloquear(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoBloquear(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<long> WebAdminCatalogoTextoBloquearAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoBloquearAsync(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public long WebAdminCatalogoTextoDesbloquear(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoDesbloquear(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<long> WebAdminCatalogoTextoDesbloquearAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoDesbloquearAsync(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public string WebAdminCatalogoTextoEsBloqueadoPor(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoEsBloqueadoPor(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<string> WebAdminCatalogoTextoEsBloqueadoPorAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoEsBloqueadoPorAsync(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public bool WebAdminCatalogoTextoEsBloqueado(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoEsBloqueado(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<bool> WebAdminCatalogoTextoEsBloqueadoAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoEsBloqueadoAsync(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> WebAdminCatalogoTextoSeleccionarTodo(string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoSeleccionarTodo(strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity>> WebAdminCatalogoTextoSeleccionarTodoAsync(string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoSeleccionarTodoAsync(strAKASesion, strAKASubCompania);
        }
        
        public System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> WebAdminCatalogoTextoSeleccionar(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoSeleccionar(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity>> WebAdminCatalogoTextoSeleccionarAsync(string SubCompania, string Texto, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoSeleccionarAsync(SubCompania, Texto, strAKASesion, strAKASubCompania);
        }
        
        public System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity> WebAdminCatalogoTextoSeleccionarXSubCompania(string SubCompania, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoSeleccionarXSubCompania(SubCompania, strAKASesion, strAKASubCompania);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.List<adminlte.AdminCatalogoTextoService.AdminCatalogoTextoEntity>> WebAdminCatalogoTextoSeleccionarXSubCompaniaAsync(string SubCompania, string strAKASesion, string strAKASubCompania) {
            return base.Channel.WebAdminCatalogoTextoSeleccionarXSubCompaniaAsync(SubCompania, strAKASesion, strAKASubCompania);
        }
    }
}
