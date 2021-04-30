/*
Copyright (C) 2020-2021 Tris Shores

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Drawing;
using System.Windows.Forms;

namespace marc_common
{
    internal class FormX : Form
    {
        internal Image v_spinner;

        internal void m_AssignIconTitle(Form v_form, string v_formName)
        {
#if PREDICTIVEBIB_UI
            v_form.Icon = predictivebib_ui.Properties.Resources.predictivebib;
            v_form.Text = v_formName;
            v_spinner = predictivebib_ui.Properties.Resources.spinner;
#elif MODMARC_UI
            v_form.Icon = modmarc_ui.Properties.Resources.modmarc;
            v_form.Text = v_formName;
            v_spinner = modmarc_ui.Properties.Resources.spinner;
#elif VIEWMARC_UI
            v_form.Icon = viewmarc_ui.Properties.Resources.viewmarc;
            v_form.Text = v_formName;
            v_spinner = viewmarc_ui.Properties.Resources.spinner;
#elif BIBMARC_UI
            v_form.Icon = bibframe_ui.Properties.Resources.bibmarc;
            v_form.Text = v_formName;
#endif
        }
    }
}