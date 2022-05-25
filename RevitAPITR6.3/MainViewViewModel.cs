using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITR6._3
{
    public class MainViewViewModel
    {
        private ExternalCommandData commandData;

        public List<FamilySymbol> FamilyTypes { get; } = new List<FamilySymbol>();
        public Pipe Pipe { get; }
        public FamilySymbol SelectedFamilyType { get; set; }
        public DelegateCommand SaveCommand { get; }

        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            Tags = TagsUtils.GetPipeTagTypes(commandData);
            Pipe = SelectionUtils.GetObject<Pipe>(_commandData, "Vash vybor");
            SaveCommand = new DelegateCommand(OnSaveCommand);
            SaveCommand = saveCommand;
        }

        private void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            var pipeLocCurve = Pipe.Location as LocationCurve;
            var pipeCurve = pipeLocCurve.Curve;
            var pipeMidPoint = (pipeCurve.GetEndPoint(0) + pipeCurve.GetEndPoint(1)) / 2;
            using (var ts=new Transaction(doc, "Create tag"))
            {
                ts.Start();
                IndependentTag.Create(doc, SelectedTagType.Id, doc.ActiveView.Id, new Reference(Pipe), false, TagOrientation.Horizontal, pipePoint);
                ts.Commit;
            }

            RaiseCloseRequest();
        }


        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}
