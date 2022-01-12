namespace imaginaryCompany.Data
{
    public class FakeData : iCompanyData
    {
        private IEnumerable<Software> ProcessData()
        {
            return SoftwareManager.GetAllSoftware();
        }

        /// <summary>
        ///  retuns software data for company
        /// </summary>
        /// <returns>IEnumerable<Software></returns>
        public IEnumerable<Software> GetData()
        {
            return ProcessData();
        }
    }
}
