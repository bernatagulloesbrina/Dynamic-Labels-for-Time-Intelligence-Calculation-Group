// '2021-07-10 / B.Agullo / 
// by Bernat Agulló
// www.esbrina-ba.com

//this script creates an extra calculation group to work together with Time Calculation group 
//you need to create the Time calculation group script with the dynamic label measures before running this script 
//the names of the measure and affected measure table must match 
//if you changed the default valures on the time intel calc group, change them heere too .

string labelsCalculationGroupName = "Labels"; 
string labelsCalculationGroupColumnName = "Label Type"; 
string labelsCalculationItemName = "Last Point Time Calculation"; 

string affectedMeasuresTableName = "Time Intelligence Affected Measures"; 
string affectedMeasuresColumnName = "Measure"; 

//add the name of the existing time intel calc group here
string calcGroupName = "Time Intelligence";

//add the name for date table of the model
string dateTableName = "Date";
string dateTableDateColumnName = "Date";

string labelAsValueMeasureName = "Label as Value Measure"; 

string flagExpression = "UNICHAR( 8204 )"; 


//generates new calc group 
var calculationGroupTable1 = Model.AddCalculationGroupTable(labelsCalculationGroupName);

//sees the default precedence number assigned 
int labelGroupPrecedence = (Model.Tables[labelsCalculationGroupName] as CalculationGroupTable).CalculationGroup.Precedence;
int timeIntelGroupPrecedence = (Model.Tables[calcGroupName] as CalculationGroupTable).CalculationGroup.Precedence;

//if time intel has lower precedence... 
if(labelGroupPrecedence > timeIntelGroupPrecedence) {
    //...swap precedence values 
    (Model.Tables[labelsCalculationGroupName] as CalculationGroupTable).CalculationGroup.Precedence = timeIntelGroupPrecedence;
    (Model.Tables[calcGroupName] as CalculationGroupTable).CalculationGroup.Precedence = labelGroupPrecedence; 
}; 


(Model.Tables["Labels"].Columns["Name"] as DataColumn).Name = labelsCalculationGroupColumnName;
var calculationItem1 = calculationGroupTable1.AddCalculationItem(labelsCalculationItemName);
calculationItem1.Expression = 
"SWITCH(" + 
"\n    TRUE()," + 
"\n    SELECTEDMEASURENAME()" + 
"\n        IN VALUES( '" + affectedMeasuresTableName + "'[" + affectedMeasuresColumnName + "] )," + 
"\n        VAR maxDateInVisual =" + 
"\n            CALCULATE( MAX( '" + dateTableName + "'[" +dateTableDateColumnName + "] ), ALLSELECTED( '" + dateTableName + "' ) )" + 
"\n        VAR maxDateInDataPoint =" + 
"\n            MAX( '" + dateTableName "'[" + dateTableDateColumnName + "] )" + 
"\n        VAR result =" + 
"\n            IF( maxDateInDataPoint = maxDateInVisual, [" + labelAsValueMeasureName +"] )" + 
"\n        RETURN" + 
"\n           " + flagExpression + " & \"\"\"\" & result & \"\"\";\"\"\" & result & \"\"\";\"\"\" & result & \"\"\";\"\"\" & result & \"\"\"\"," + 
"\n    SELECTEDMEASUREFORMATSTRING()" + 
"\n)";
