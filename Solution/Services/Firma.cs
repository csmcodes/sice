using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using java.security;
using java.io;
using java.util;
using java.security.cert;
using javax.xml.parsers;
using es.mityc.javasign.pkstore;
using es.mityc.javasign.pkstore.keystore;
using es.mityc.javasign.trust;
using es.mityc.javasign.xml.xades.policy;
using es.mityc.firmaJava.libreria.xades;
using es.mityc.javasign.xml.refs;
using es.mityc.firmaJava.libreria.utilidades;
using org.w3c.dom;
using sviudes.blogspot.com;
using es.mityc.firmaJava.role;




namespace Services
{
    public class Firma
    {


        private static Document LoadXML(string path)
        {
            DocumentBuilderFactory dbf = DocumentBuilderFactory.newInstance();
            dbf.setNamespaceAware(true);
            return dbf.newDocumentBuilder().parse(new BufferedInputStream(new FileInputStream(path)));
        }


        private static X509Certificate LoadCertificate(string path, string password, out PrivateKey privateKey, out Provider provider, int? indice)
        {
            X509Certificate certificate = null;
            provider = null;
            privateKey = null;

            //Cargar certificado de fichero PFX  
            KeyStore ks = KeyStore.getInstance("PKCS12");
            ks.load(new BufferedInputStream(new FileInputStream(path)), password.ToCharArray());
            IPKStoreManager storeManager = new KSStore(ks, new PassStoreKS(password));
            List certificates = storeManager.getSignCertificates();

            //Si encontramos el certificado...  
            //if (certificates.size() == 1)

            // {

            if (!indice.HasValue)
                indice = 0;

            certificate = (X509Certificate)certificates.get(indice.Value);

            // Obtención de la clave privada asociada al certificado  
            privateKey = storeManager.getPrivateKey(certificate);

            // Obtención del provider encargado de las labores criptográficas  
            provider = storeManager.getProvider(certificate);
            //}

            return certificate;
        }


        public static void sign(string path)
        {
            PrivateKey privateKey;
            Provider provider;
            //X509Certificate certificate = LoadCertificate("c:\\temp\\certificado.pfx", "contraseña", out privateKey, out provider);
            string pathfirma = HttpContext.Current.Server.MapPath("../firmas");

            X509Certificate certificate = LoadCertificate(pathfirma + "\\diego_adrian_pineda_espinoza.p12", "Barcelona", out privateKey, out provider, null);
            //X509Certificate certificate = LoadCertificate("c:\\temp\\certificado.pfx", "contraseña", out privateKey, out provider);
            //X509Certificate certificate = LoadCertificate(pathfirma + "\\glenda_magali_armijos_fajardo.p12", "Dayana2006", out privateKey, out provider);

            //Si encontramos el certificado...  
            if (certificate != null)
            {
                //Política de firma (Con las librerías JAVA, esto se define en tiempo de ejecución)  
                TrustFactory.instance = es.mityc.javasign.trust.TrustExtendFactory.newInstance();
                TrustFactory.truster = es.mityc.javasign.trust.MyPropsTruster.getInstance();
                PoliciesManager.POLICY_SIGN = new es.mityc.javasign.xml.xades.policy.facturae.Facturae31Manager();
                PoliciesManager.POLICY_VALIDATION = new es.mityc.javasign.xml.xades.policy.facturae.Facturae31Manager();




                DataToSign dataToSign = new DataToSign();
                dataToSign.setXadesFormat(EnumFormatoFirma.XAdES_BES);
                dataToSign.setEsquema(XAdESSchemas.XAdES_132);
                dataToSign.setPolicyKey("factura");
                dataToSign.setAddPolicy(true);
                dataToSign.setXMLEncoding("UTF-8");
                dataToSign.addClaimedRol(new SimpleClaimedRole("Rol de firma"));
                dataToSign.setEnveloped(true);
                //dataToSign.addObject(new ObjectToSign(new AllXMLToSign(), "Documento de ejemplo",  null, "text/xml", null));
                dataToSign.addObject(new ObjectToSign(new InternObjectToSign("comprobante"), "Documento de ejemplo", null, "text/xml", null));
                dataToSign.setParentSignNode("titulo");
                dataToSign.setDocument(LoadXML(path));

                //Crear datos a firmar  
                /*DataToSign dataToSign = new DataToSign();
                dataToSign.setXadesFormat(EnumFormatoFirma.XAdES_BES); //XAdES-EPES                                  
                dataToSign.setEsquema(XAdESSchemas.XAdES_132);
                dataToSign.setPolicyKey("facturae31"); //Da igual lo que pongamos aquí, la política de firma se define arriba  
                dataToSign.setAddPolicy(true);
                dataToSign.setXMLEncoding("UTF-8");
                dataToSign.setEnveloped(true);
                dataToSign.addObject(new ObjectToSign(new AllXMLToSign(), "Descripcion del documento", null, "text/xml", null));                
                dataToSign.setDocument(LoadXML(path));*/

                //Firmar  
                Object[] res = new FirmaXML().signFile(certificate, dataToSign, privateKey, provider);

                // Guardamos la firma a un fichero en el home del usuario  
                UtilidadTratarNodo.saveDocumentToOutputStream((Document)res[0], new FileOutputStream("c:\\Temp\\signed.xml"), true);
            }
        }


        public static void signxml(string path, int empresa, string numero, string certificado, string password, string pathcertificado, int? indice)
        {

            PrivateKey privateKey;
            Provider provider;
            //X509Certificate certificate = LoadCertificate("c:\\temp\\certificado.pfx", "contraseña", out privateKey, out provider);

            string pathfirma = path + "\\" + pathcertificado;
            string pathxml = path + "\\temp\\" + numero + "_" + empresa + ".xml";

            X509Certificate certificate = LoadCertificate(pathfirma + "\\" + certificado, password, out privateKey, out provider, indice);
            //Si encontramos el certificado...  
            if (certificate != null)
            {
                //Política de firma (Con las librerías JAVA, esto se define en tiempo de ejecución)  
                TrustFactory.instance = es.mityc.javasign.trust.TrustExtendFactory.newInstance();
                TrustFactory.truster = es.mityc.javasign.trust.MyPropsTruster.getInstance();
                PoliciesManager.POLICY_SIGN = new es.mityc.javasign.xml.xades.policy.facturae.Facturae31Manager();
                PoliciesManager.POLICY_VALIDATION = new es.mityc.javasign.xml.xades.policy.facturae.Facturae31Manager();

                DataToSign dataToSign = new DataToSign();
                dataToSign.setXadesFormat(EnumFormatoFirma.XAdES_BES);
                dataToSign.setEsquema(XAdESSchemas.XAdES_132);
                dataToSign.setPolicyKey("factura");
                dataToSign.setAddPolicy(true);
                dataToSign.setXMLEncoding("UTF-8");
                dataToSign.addClaimedRol(new SimpleClaimedRole("Rol de firma"));
                dataToSign.setEnveloped(true);
                //dataToSign.addObject(new ObjectToSign(new AllXMLToSign(), "Documento de ejemplo", null, "text/xml", null));
                dataToSign.addObject(new ObjectToSign(new InternObjectToSign("comprobante"), "Documento de ejemplo", null, "text/xml", null));
                dataToSign.setParentSignNode("comprobante");
                dataToSign.setDocument(LoadXML(pathxml));
                //Crear datos a firmar  
                /*DataToSign dataToSign = new DataToSign();
                dataToSign.setXadesFormat(EnumFormatoFirma.XAdES_BES); //XAdES-EPES                                  
                dataToSign.setEsquema(XAdESSchemas.XAdES_132);
                dataToSign.setPolicyKey("facturae31"); //Da igual lo que pongamos aquí, la política de firma se define arriba  
                dataToSign.setAddPolicy(true);
                dataToSign.setXMLEncoding("UTF-8");
                dataToSign.setEnveloped(true);
                dataToSign.addObject(new ObjectToSign(new AllXMLToSign(), "Descripcion del documento", null, "text/xml", null));                
                dataToSign.setDocument(LoadXML(path));*/

                //Firmar  
                Object[] res = new FirmaXML().signFile(certificate, dataToSign, privateKey, provider);

                FileOutputStream a = new FileOutputStream(pathxml);
                // Guardamos la firma a un fichero en el home del usuario  
                UtilidadTratarNodo.saveDocumentToOutputStream((Document)res[0], a, true);

                a.close();


            }

        }



    }
}
