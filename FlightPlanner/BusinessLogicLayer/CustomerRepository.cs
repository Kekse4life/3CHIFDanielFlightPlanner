using FlightPlanner.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightPlanner.BusinessLogicLayer
{
    public class CustomerRepository
    {
        private CustomerDataMapper _customerMapper;
        private BookingDataMapper _bookingMapper;

        public CustomerRepository(string connectionString)
        {
            _customerMapper = new CustomerDataMapper(connectionString);
            _bookingMapper = new BookingDataMapper(connectionString);
        }

        public void DeleteCustomerAndHisBookings(int customerId)
        {
            _bookingMapper.DeleteByCustomerId(customerId);

            _customerMapper.Delete(customerId);
        }
    }
}
