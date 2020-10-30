using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

namespace accounts.Model
{
    class Trsansactions
    {
        public DateTime Tr_date { get; set; }
        public int Ac_Id { get; set; }
        public int Op_Ac_Id { get; set; }
        public int Id { get; set; }
        public double Dr_Amount { get; set; }
        public double Cr_Amount { get; set; }
        public int Eno { get; set; }
        public string Entry { get; set; }
        public string Cinv_no { get; set; }
    }
    class SCompany
    {
        public string company { get; set; }
        public string SoftWareCaption { get; set; }
        public string lmark { get; set; }
        public string place { get; set; }
        public string street { get; set; }
        public string post { get; set; }
        public string zipcode { get; set; }
        public string TAXID { get; set; }
        public string DLNO { get; set; }
        public string expno { get; set; }
        public string email { get; set; }
        public string officeno { get; set; }
        public string Mobile { get; set; }
        public DateTime f_date1 { get; set; }
        public DateTime f_date2 { get; set; }
    }
    class GroupModel
    {
        public int ID { get; set; }
        public int P_ID { get; set; }
        public GroupModel ParentGroup { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Catagory { get; set; }
        public double Max_Disc { get; set; }
        public double Cr_Loc { get; set; }
        public double Dr_Loc { get; set; }


    }
    class PayrollEntryVoucher
    {
        public int Id { get; set; }
        public int PayrollNo { get; set; }
        public bool IsGenerated { get; set; }
        public string Voucher { get; set; }
        public double Amount { get; set; }

    }
    class PayRollEntryModel
    {
        public DateTime DATE { get; set; }
        public int VNO { get; set; }
        public string Employee { get; set; }
        public int EID { get; set; }
        public double WHs { get; set; }
        public double WDs { get; set; }  
        public double Basic { get; set; }
        public PayrollEntryVoucher Allownaces { get; set; }
        public PayrollEntryVoucher Commission { get; set; }
        public PayrollEntryVoucher Advance { get; set; }
        public PayrollEntryVoucher Deductions { get; set; }
        public string Narration { get; set; }
        public int DrAcid { get; set; }
        public EmployeeModel DrAccount { get; set; }
        public AccountModel CrAccount { get; set; }
        public double Total { get; set; }
        public bool IsVoucher { get; set; }
        public bool IsRecurring { get; set; }
        public int Task_ID { get; set; }



    }
    class TempClass
    {

        public string stringVal { get; set; }
    }
    class Task
    {
        public int ID { get; set; }
        public int ENO { get; set; }
        public string ENTRY { get; set; }
        public string T_LABEL { get; set; }
        public decimal T_AMOUNT { get; set; }


         
    }
    class CashBookModel
    {

        public DateTime Date { get; set; }
        public string Catagory { get; set; }
        
     
        public AccountModel DrAccount { get; set; }
        public AccountModel CrAccount { get; set; }   
        
        public double Dr_Amount { get; set; }
        public double Cr_Amount { get; set; }
        public string Voucher { get; set; }        
        public int VNo { get; set; }         
        public string Invno { get; set; }
        public double Balance { get; set; }
        public double Opening { get; set; }
        public string Narration { get; set; }



    }
    class ReceiptModel
    {
        public DateTime Date { get; set; }
        public bool isRecurr { get; set; }

        public string Invno { get; set; }
        public double InvBalance { get; set; }
        public int? Task_Id { get; set; }
        public string Dr_Amount { get; set; }
        public AccountModel CrAccount { get; set; }
        public AccountModel DrAccount { get; set; }
        public double DrAmount { get; set; }
        public double DiscP { get; set; }
        public double DAmount { get; set; }
        public int rno { get; set; }
        public string Narration { get; set; }

    }
    class BankReceiptModel
    {
        public string Check_Account { get; set; }
        public DateTime Date { get; set; }
        
        public AccountModel DrAccount { get; set; }
        public AccountModel CrAccount { get; set; }
        
        public string Type { get; set; }
        public string CheqNo { get; set; }
        public DateTime CheqDate { get; set; }
        public double Amount { get; set; }
        public double BankCharge { get; set; }
        public double DiscP { get; set; }
        public double DiscAmount { get; set; }
 
   
        public int rnoCheck { get; set; }
        public int rno { get; set; }
    
        public string Narration { get; set; }
        public string Status { get; set; }
        public string Invno { get; set; }
        public double InvBalance { get; set; }
    }
    class JournalModel

    {
        //public string Check_Account { get; set; }
        public DateTime Date { get; set; }
        public AccountModel DrAccount { get; set; }
        public AccountModel CrAccount { get; set; }
        //public string Dr_Account { get; set; }
       
        public int? Task_Id { get; set; }
        public double Dr_Amount { get; set; }
        //public string Cr_Account { get; set; }
        
        public double Cr_Amount { get; set; }
        public int jno { get; set; }
        public string Narration { get; set; }
        public string Invoice { get; set; }
        public bool Isrecurring { get; set; }
 






    }
    class AccountModel : accounts.ModelViews.ViewModelBase
    {
        public AccountModel()
        {
            ID = 0;
            Name = "";
            Short_Name = "";
            City = "";
            Address = "";
            Catagory = "None";
            PhoneNo = "";
            Mob = "";
            CrLimit = 0;
            DrLimit = 0;
            MaxDisc = 0;
            Balance = 0;
            ParentGroup = 0;
        }
        bool _ischecked = false; 
        public bool IsChecked
        {
            get => _ischecked;
            set => SetProperty(ref _ischecked, value);
        }
        public int ID { get; set; }
        public GroupModel Parent { get; set; }
        public string Name { get; set; }
        public string Short_Name { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        //public string Group { get; set; }
      
        public string Catagory { get; set; }
        public string PhoneNo { get; set; }
        public string Mob { get; set; }
        public double CrLimit { get; set; }
        public double DrLimit { get; set; }
        public double MaxDisc { get; set; }
        public double Balance { get; set; }
        public string DueDate { get; set; }
        public int ParentGroup { get; set; }







    }
    class BankPaymentModel

    {

        public DateTime Date { get; set; }
        public AccountModel DrAccount { get; set; }
        public AccountModel CrAccount { get; set; }
        public double Amount { get; set; }
        public int pno { get; set; }
        public string Type { get; set; }
        public string CheqNo { get; set; }
        public DateTime CheqDate { get; set; }
        public int Crid { get; set; }
        public int Drid { get; set; }
        public string status { get; set; }
        public double Disc { get; set; }
        public double DiscAmount { get; set; }
        public double BankCharge { get; set; }
        public string Narration { get; set; }

        public string Invno { get; set; }
        public double InvBalance { get; set; }
 

    }
    class PaymentModel
    {
        public DateTime Date { get; set; }
       
        public AccountModel DrAccount { get; set; }
        
        public AccountModel CrAccount { get; set; }
        public double Amount { get; set; }
        
        public double Disc { get; set; }
        public bool IsRecurring { get; set; }
        public double DiscAmount { get; set; }
        public string Narration { get; set; }
        public int pno { get; set; }
        public int? Task_Id { get; set; }

        
        public string Invno { get; set; }
        public double InvBalance { get; set; }
    }
    class PayRollVoucherModel
    {
        public int VNO { get; set; }         
        public bool Isrecurring { get; set; }
        public DateTime PPDate { get; set; }        
        public EmployeeModel DrAccount { get; set; }
        
        public AccountModel CrAccount { get; set; }
        public int Task_ID { get; set; }
        public double Amount { get; set; }
        public string Narration { get; set; }
        public string VoucherType { get; set; }
         






    }
    class ValueHolder
    {
        public string Value1 { get; set; }
    }
    class EmployeeModel
    {
        public AccountModel Account { get; set; }
        public DepartmentModel Department { get; set; }
        public int Id { get; set; }  
        
        public string Desig { get; set; }        
        public string Emp_Id { get; set; }        
        public double Basic { get; set; }
        public double Comm { get; set; }
        public DateTime DOJ { get; set; }
        public bool IsDailyWager { get; set; }
    }

    class DepartmentModel
    {
        public int Dep_id { get; set; }
        public string Narration { get; set; }
        public string Name { get; set; }
        public string Dep_Head { get; set; }

    }
}
