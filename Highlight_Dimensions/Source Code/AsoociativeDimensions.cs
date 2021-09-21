namespace Highlight_Dimensions
{
    using NXOpen;
    using NXOpen.Annotations;
    using NXOpen.Drawings;
    using NXOpen.UF;
    using NXOpen.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class AsoociativeDimensions
    {
        /// <summary>
        /// Dictionary property to store all sheets in the NX workpart.
        /// </summary>
        public static Dictionary<int, DrawingSheet> allSheetDict = new Dictionary<int, DrawingSheet>();

        /// <summary>
        /// Getting dictionary of all sheets.
        /// </summary>
        /// <returns>Dictionary of all sheets.</returns>
        public static Dictionary<int, DrawingSheet> GetDrawingSheets()
        {
            try
            {
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;
                DrawingSheet[] sheetCollection = workPart.DrawingSheets.ToArray();
                int key = 1;
                foreach (DrawingSheet onesheet in sheetCollection)
                {
                    allSheetDict[key] = onesheet;
                    key++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return allSheetDict;
        }

        /// <summary>
        /// Getting all the dimensions in the selected sheet and filtering
        /// them to get the dimensions associated with the geometrics.
        /// </summary>
        /// <param name="dictKey">Key to access sheet from dictionary of sheets.</param>
        public static void HighlightAssociativeDimension(int dictKey)
        {
            try
            {
                DrawingSheet sheet = allSheetDict[dictKey];
                Session theSession = Session.GetSession();
                Part workPart = theSession.Parts.Work;

                sheet.Open();
                sheet.ActivateForSketching();
                
                // Using Ufunc api to get all the visible object in the current view sheet.
                Tag[] allSheetVisibleObjects, clipped;
                int n_visible, n_clipped;
                UFSession ufSession = UFSession.GetUFSession();
                ufSession.View.AskVisibleObjects(sheet.View.Tag, out n_visible, out allSheetVisibleObjects, out n_clipped, out clipped);

                // Getting all the dimensions in the selected.
                List<Dimension> dimensions = new List<Dimension>();
                foreach (DisplayableObject disObj in sheet.View.AskVisibleObjects())
                {
                    if (disObj is Dimension)
                    {
                        dimensions.Add((Dimension)disObj);
                    }
                }

                // Shows warning if the selected sheet has no dimensions.
                if (dimensions.Count() == 0)
                {
                    string title = "No dimension found";
                    string message = "The sheet does not contain any dimension. Operation might fail.";
                    NXMessageBox.DialogType dialogType = NXMessageBox.DialogType.Warning;
                    NXOpen.UI.GetUI().NXMessageBox.Show(title, dialogType, message);
                }

                List<Tag> allAssociativeTag = new List<Tag>();
                List<Tag> allTempNonAssocaitiveTag = new List<Tag>();
                List<Tag> allNonAssocaitiveTag = new List<Tag>();
                foreach (Dimension dim in dimensions)
                {
                    int noOfAssociate = dim.NumberOfAssociativities;
                    // If number of associatives for a dimensions is zero.
                    // Then the dimensions is stored in allTempNonAssocaitiveTag list.
                    if (noOfAssociate == 0)
                    {
                        allTempNonAssocaitiveTag.Add(dim.Tag);
                        break;
                    }
                    for (int i = 1; i <= noOfAssociate; i++)
                    {
                        Associativity assoc = dim.GetAssociativity(i);
                        NXObject firstAssoc = assoc.FirstObject;
                        NXObject secondAssoc = assoc.SecondObject;

                        // If first associative of a dimension is not null and is a displayable object condition
                        if (firstAssoc != null && (firstAssoc is DisplayableObject))
                        {
                            // If dimension is associative but not present in allSheetVisibleObjects
                            // it means that dimension is associative but not from current sheet
                            // so we store it's tag in allAssociativeTag list,
                            // else it's tag is stored in allTempNonAssocaitiveTag list.
                            if (!allSheetVisibleObjects.Contains(firstAssoc.Tag))
                            {
                                allAssociativeTag.Add(dim.Tag);
                                break;
                            }
                            else
                            {
                                allTempNonAssocaitiveTag.Add(dim.Tag);
                            }
                        }

                        // If second associative of a dimension is not null and is a displayable object condition
                        if (secondAssoc != null && (secondAssoc is DisplayableObject))
                        {
                            // If dimension is associative but not present in allSheetVisibleObjects
                            // it means that dimension is associative but not from current sheet 
                            // so we store it's tag in allAssociativeTag list,
                            // else it's tag is stored in allTempNonAssocaitiveTag list.
                            if (!allSheetVisibleObjects.Contains(secondAssoc.Tag))
                            {
                                allAssociativeTag.Add(dim.Tag);
                                break;
                            }
                            else
                            {
                                allTempNonAssocaitiveTag.Add(dim.Tag);
                            }
                        }
                    }

                    // If a dimension is not present in allTempNonAssocaitiveTag list or allAssociativeTag list
                    // then we store it's tag in allTempNonAssocaitiveTag
                    if (!allTempNonAssocaitiveTag.Contains(dim.Tag) && !allAssociativeTag.Contains(dim.Tag))
                    {
                        allTempNonAssocaitiveTag.Add(dim.Tag);
                    }
                }
                // It a tag is stored twice, we remove the duplicates
                allAssociativeTag = allAssociativeTag.Distinct().ToList();
                allTempNonAssocaitiveTag = allTempNonAssocaitiveTag.Distinct().ToList();

                // If a dimensions's tag in allTempNonAssocaitiveTag is not present in allAssociativeTag
                // it means that dimension is non associative and we store it in allNonAssociativeTag
                foreach (Tag oneTag in allTempNonAssocaitiveTag)
                {
                    if (!allAssociativeTag.Contains(oneTag))
                    {
                        allNonAssocaitiveTag.Add(oneTag);
                    }
                }
                
                // Displays total count of non-associated dimensions in the selected sheet.
                if (allNonAssocaitiveTag.Count > 0)
                {
                    string title = "Non-Associative dimension";
                    string message = string.Format("The selected sheet have {0} associated and {1} non-associative dimensions.", allAssociativeTag.Count.ToString(), allNonAssocaitiveTag.Count.ToString());
                    NXMessageBox.DialogType dialogType = NXMessageBox.DialogType.Information;
                    NXOpen.UI.GetUI().NXMessageBox.Show(title, dialogType, message);
                }
                else
                {
                    string title = "Associative dimension";
                    string message = string.Format("The selected sheet have {0} associative dimensions.", allAssociativeTag.Count.ToString());
                    NXMessageBox.DialogType dialogType = NXMessageBox.DialogType.Information;
                    NXOpen.UI.GetUI().NXMessageBox.Show(title, dialogType, message);
                }

                // Highlights the dimensions associated with the geometrics.
                foreach (Tag oneAssTag in allAssociativeTag)
                {
                    Dimension assocDim = (Dimension)NXObjectManager.Get(oneAssTag);
                    assocDim.Highlight();
                    
                }
                sheet.View.UpdateDisplay();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}