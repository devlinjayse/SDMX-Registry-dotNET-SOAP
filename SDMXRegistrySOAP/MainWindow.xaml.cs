using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace SDMXRegistrySOAP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGetConceptScheme_Click(object sender, RoutedEventArgs e)
        {
            // will call GetConceptScheme

            // create the SOAP Client
            SDMXRegSvc.SDMXRegistrySOAPPortTypeClient regClient = new SDMXRegSvc.SDMXRegistrySOAPPortTypeClient();
            // create the ConceptSchemeQuery
            SDMXRegSvc.ConceptSchemeQueryType conceptQuery = new SDMXRegSvc.ConceptSchemeQueryType();
            // create the Query Response
            SDMXRegSvc.GetConceptSchemeResponse conceptResponse = new SDMXRegSvc.GetConceptSchemeResponse();
            // create a Structure for the Response
            SDMXRegSvc.StructureType respStructure = new SDMXRegSvc.StructureType();
            // set the Structure as the Response's Structure object
            respStructure = conceptResponse.Structure;
            // create objects to hold the XML Elements we need to construct the Query
            XmlElement xQuery, xAgency, xReturnDetails, xConcept, xReferences, xVersion;
            // create an XML Document from which to generate elements
            XmlDocument xDoc = new XmlDocument();
            // define the Query element
            xQuery = xDoc.CreateElement("Query", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/message");
            // define the Version element
            xVersion = xDoc.CreateElement("Version", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query");
            // define the References element
            xReferences = xDoc.CreateElement("References", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query");
            // define the attributes for References
            xReferences.Attributes.Append(xDoc.CreateAttribute("detail"));
            xReferences.Attributes.Append(xDoc.CreateAttribute("processConstraints"));
            // set the attribute values
            xReferences.Attributes.GetNamedItem("detail").Value = "Full";
            xReferences.Attributes.GetNamedItem("processConstraints").Value = "false";
            // define the ReturnDetails element
            xReturnDetails = xDoc.CreateElement("ReturnDetails", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query");
            // define the attributes for ReturnDetails element
            xReturnDetails.Attributes.Append(xDoc.CreateAttribute("detail"));
            // set the attribute value
            xReturnDetails.Attributes.GetNamedItem("detail").Value = "Full";
            // add References as a child of ReturnDetails
            xReturnDetails.AppendChild(xReferences);
            // create the Agency element
            xAgency = xDoc.CreateElement("AgencyID", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query");
            // set the Agency value
            xAgency.InnerText = "SNZ";
            // create and populate the operator attribute of Agency
            xAgency.Attributes.Append(xDoc.CreateAttribute("operator"));
            xAgency.Attributes.GetNamedItem("operator").Value = "equal";
            // create the ConceptSchemeWhere element to hold the key query clause
            xConcept = xDoc.CreateElement("ConceptSchemeWhere", "http://www.sdmx.org/resources/sdmxml/schemas/v2_1/query");
            // create and set the type attribute
            xConcept.Attributes.Append(xDoc.CreateAttribute("type"));
            xConcept.Attributes.GetNamedItem("type").Value = "ConceptScheme";
            // build up the Query element
            // add the Version element as a child to the ConceptSchemeWhere element
            xConcept.AppendChild(xVersion);
            // add the AgencyID element as a child to the ConceptSchemeWhere element
            xConcept.AppendChild(xAgency);
            // add the ReturnDetails element as a child to the Query element
            xQuery.AppendChild(xReturnDetails);
            // add the ConceptSchemeWhere element as a child to the Query element
            xQuery.AppendChild(xConcept);
            // create and set the Sender for the Header
            SDMXRegSvc.SenderType qSender = new SDMXRegSvc.SenderType();
            qSender.id = "SNZ";

            // create the Header object and set the appropriate properties
            SDMXRegSvc.BasicHeaderType qHdr = new SDMXRegSvc.BasicHeaderType();
            qHdr.ID = "JDTESTQRY01";
            qHdr.Test = true;
            qHdr.Sender = qSender;

            // set the query body header object
            conceptQuery.Header = qHdr;
            // re size the Any array of the query body so we can asign the Query payload
            var arrBody = conceptQuery.Any;
            Array.Resize<XmlElement>(ref arrBody, 1);
            // set the first element of the Any array to be the Query element defined above
            conceptQuery.Any = arrBody;
            conceptQuery.Any.SetValue(xQuery, 0);

            // create the Try Catch block in order to call the web service and catch any exceptions
            try
            {
                // set the response Structure to contain the result of the GetConceptScheme call (passing the query body that we've constructed)
                respStructure = regClient.GetConceptScheme(conceptQuery);
                // open a message box containing the raw xml stream of the response body.
                MessageBox.Show(respStructure.Any.First().InnerText);
            }
            catch (System.ServiceModel.CommunicationException ex)
            {
                // catch any CommunicationException and open a message box containing the error message
                MessageBox.Show(ex.InnerException.Message);
            }
            

        }
    }
}
